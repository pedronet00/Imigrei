// Imigrei.Infra/Storage/ArquivoStorageLocal.cs
using Imigrei.Application.Common.Storage;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Imigrei.Infra.Storage;

/// <summary>
/// Grava os documentos no sistema de arquivos — a pasta local em
/// desenvolvimento, um volume da VPS em produção. A raiz vem de
/// <c>Storage:ImigrantesPath</c>, então a troca é só de configuração.
/// </summary>
public class ArquivoStorageLocal : IArquivoStorage
{
    private const int TamanhoBuffer = 81920;

    private readonly string _raizArmazenamento;

    public ArquivoStorageLocal(IOptions<StorageSettings> settings)
    {
        var caminhoConfigurado = settings.Value.ImigrantesPath;

        var raiz = string.IsNullOrWhiteSpace(caminhoConfigurado)
            ? Path.Combine(AppContext.BaseDirectory, "Storage", "Imigrantes")
            : caminhoConfigurado;

        // Resolvido uma vez para que as checagens de contenção comparem
        // caminhos absolutos e normalizados.
        _raizArmazenamento = Path.GetFullPath(raiz);

        Directory.CreateDirectory(_raizArmazenamento);
    }

    public async Task<ArquivoArmazenado> SalvarAsync(
        Stream conteudo, string nomeOriginal, Guid imigranteId,
        long tamanhoMaximoBytes, CancellationToken cancellationToken = default)
    {
        // O nome gravado é opaco: nada do nome enviado pelo usuário alcança o
        // disco, o que elimina traversal e colisão de nomes de uma vez.
        var extensao = Path.GetExtension(nomeOriginal).ToLowerInvariant();
        var nomeArmazenado = $"{Guid.NewGuid():N}{extensao}";

        var pastaImigrante = Path.Combine(_raizArmazenamento, imigranteId.ToString());
        Directory.CreateDirectory(pastaImigrante);

        var caminhoCompleto = Path.Combine(pastaImigrante, nomeArmazenado);

        // Separador "/" sempre: o caminho fica no banco e precisa continuar
        // válido se o deploy migrar do Windows para a VPS Linux.
        var caminhoRelativo = $"{imigranteId}/{nomeArmazenado}";

        long totalBytes = 0;
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        try
        {
            await using (var destino = NovoArquivoRestrito(caminhoCompleto))
            {
                var buffer = new byte[TamanhoBuffer];
                int lidos;

                while ((lidos = await conteudo.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    totalBytes += lidos;

                    // Corta o fluxo se o conteúdo real passar do teto, mesmo que
                    // o Content-Length declarado dissesse o contrário.
                    if (totalBytes > tamanhoMaximoBytes)
                        throw new DomainException("Arquivo excede o tamanho máximo permitido.");

                    hash.AppendData(buffer, 0, lidos);
                    await destino.WriteAsync(buffer.AsMemory(0, lidos), cancellationToken);
                }
            }
        }
        catch
        {
            // Escrita interrompida não deixa arquivo pela metade em disco.
            ExcluirCaminhoCompleto(caminhoCompleto);
            throw;
        }

        if (totalBytes == 0)
        {
            ExcluirCaminhoCompleto(caminhoCompleto);
            throw new DomainException("Arquivo vazio.");
        }

        return new ArquivoArmazenado(
            nomeArmazenado, caminhoRelativo, totalBytes,
            Convert.ToHexString(hash.GetHashAndReset()));
    }

    public Task<Stream> AbrirLeituraAsync(string caminhoRelativo, CancellationToken cancellationToken = default)
    {
        var caminhoCompleto = ResolverDentroDaRaiz(caminhoRelativo);

        if (!File.Exists(caminhoCompleto))
            throw new DomainException("Arquivo não encontrado no armazenamento.");

        Stream stream = new FileStream(
            caminhoCompleto, FileMode.Open, FileAccess.Read, FileShare.Read,
            TamanhoBuffer, useAsync: true);

        return Task.FromResult(stream);
    }

    public void Excluir(string caminhoRelativo)
        => ExcluirCaminhoCompleto(ResolverDentroDaRaiz(caminhoRelativo));

    /// <summary>
    /// Converte o caminho relativo em absoluto e recusa qualquer resultado que
    /// escape da raiz. O valor vem do banco, mas defesa em profundidade é o que
    /// mantém "../" longe de arquivos sigilosos.
    /// </summary>
    private string ResolverDentroDaRaiz(string caminhoRelativo)
    {
        if (string.IsNullOrWhiteSpace(caminhoRelativo))
            throw new DomainException("Caminho de arquivo inválido.");

        // Aceita registros gravados com "\" antes da normalização para "/".
        var normalizado = caminhoRelativo
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);

        if (Path.IsPathRooted(normalizado))
            throw new DomainException("Caminho de arquivo inválido.");

        var caminhoCompleto = Path.GetFullPath(Path.Combine(_raizArmazenamento, normalizado));

        var raizComSeparador = _raizArmazenamento.EndsWith(Path.DirectorySeparatorChar)
            ? _raizArmazenamento
            : _raizArmazenamento + Path.DirectorySeparatorChar;

        if (!caminhoCompleto.StartsWith(raizComSeparador, StringComparison.Ordinal))
            throw new DomainException("Caminho de arquivo inválido.");

        return caminhoCompleto;
    }

    /// <summary>
    /// Cria o arquivo legível apenas pelo dono. No Windows a ACL herdada da
    /// pasta é quem manda, então a restrição fica no diretório raiz.
    /// </summary>
    private static FileStream NovoArquivoRestrito(string caminhoCompleto)
    {
        var opcoes = new FileStreamOptions
        {
            Mode = FileMode.CreateNew,
            Access = FileAccess.Write,
            Share = FileShare.None,
            BufferSize = TamanhoBuffer,
            Options = FileOptions.Asynchronous,
        };

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            opcoes.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;

        return new FileStream(caminhoCompleto, opcoes);
    }

    private static void ExcluirCaminhoCompleto(string caminhoCompleto)
    {
        if (File.Exists(caminhoCompleto))
            File.Delete(caminhoCompleto);
    }
}
