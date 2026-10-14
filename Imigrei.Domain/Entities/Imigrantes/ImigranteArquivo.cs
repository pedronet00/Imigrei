// Imigrei.Domain/Entities/Imigrantes/ImigranteArquivo.cs
using Imigrei.Domain.Enums;
using Imigrei.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Imigrei.Domain.Entities.Imigrantes;

/// <summary>
/// Documento sigiloso vinculado a um imigrante. O conteúdo vive no storage;
/// aqui ficam os metadados e o hash que prova que o byte a byte não mudou.
/// </summary>
public class ImigranteArquivo
{
    public static readonly IReadOnlyList<string> ExtensoesPermitidas =
        new[] { ".pdf", ".jpg", ".jpeg", ".png", ".webp" };

    public const long TamanhoMaximoBytes = 10 * 1024 * 1024; // 10 MB

    public Guid Id { get; private set; }

    public Guid ImigranteId { get; private set; }
    public Imigrante Imigrante { get; private set; } = null!;

    /// <summary>Nome que o usuário enviou. Nunca é usado como caminho em disco.</summary>
    public string NomeOriginal { get; private set; } = string.Empty;

    /// <summary>Nome opaco sob o qual o arquivo foi gravado, sem relação com o original.</summary>
    public string NomeArmazenado { get; private set; } = string.Empty;

    public string CaminhoRelativo { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long TamanhoBytes { get; private set; }

    /// <summary>SHA-256 do conteúdo, em hexadecimal maiúsculo (64 caracteres).</summary>
    public string HashSha256 { get; private set; } = string.Empty;

    public TipoArquivoImigranteEnum TipoArquivo { get; private set; }

    public DateTime DataUpload { get; private set; }

    protected ImigranteArquivo() { }

    public ImigranteArquivo(
        Guid imigranteId,
        string nomeOriginal,
        string nomeArmazenado,
        string caminhoRelativo,
        string contentType,
        long tamanhoBytes,
        string hashSha256,
        TipoArquivoImigranteEnum tipoArquivo)
    {
        ImigranteId = imigranteId;
        NomeOriginal = nomeOriginal;
        NomeArmazenado = nomeArmazenado;
        CaminhoRelativo = caminhoRelativo;
        ContentType = contentType;
        TamanhoBytes = tamanhoBytes;
        HashSha256 = hashSha256;
        TipoArquivo = tipoArquivo;
        DataUpload = DateTime.UtcNow;

        Validate();

        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Regras que podem ser checadas antes de gravar qualquer byte em disco.
    /// Existe para que o serviço rejeite o upload sem I/O desnecessário, sem
    /// reescrever as regras fora do domínio.
    /// </summary>
    public static void ValidarAntesDeArmazenar(string nomeOriginal, long tamanhoBytes)
    {
        if (string.IsNullOrWhiteSpace(nomeOriginal))
            throw new DomainException("Nome original do arquivo não pode ser vazio.");

        if (nomeOriginal.Length > 260)
            throw new DomainException("Nome original do arquivo é muito longo.");

        var extensao = Path.GetExtension(nomeOriginal).ToLowerInvariant();
        if (string.IsNullOrEmpty(extensao) || !ExtensoesPermitidas.Contains(extensao))
            throw new DomainException(
                $"Extensão de arquivo não permitida. Use: {string.Join(", ", ExtensoesPermitidas)}.");

        if (tamanhoBytes <= 0)
            throw new DomainException("Tamanho do arquivo inválido.");

        if (tamanhoBytes > TamanhoMaximoBytes)
            throw new DomainException("Arquivo excede o tamanho máximo de 10 MB.");
    }

    /// <summary>
    /// Confere o hash recalculado sobre o conteúdo lido do storage contra o
    /// registrado no upload. Falha significa arquivo corrompido ou adulterado
    /// depois de gravado — o conteúdo não deve ser entregue.
    /// </summary>
    public void GarantirIntegridade(string hashRecalculado)
    {
        var iguais = !string.IsNullOrWhiteSpace(hashRecalculado) &&
            CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(hashRecalculado),
                Encoding.ASCII.GetBytes(HashSha256));

        if (!iguais)
            throw new IntegridadeArquivoException(Id);
    }

    private void Validate()
    {
        if (ImigranteId == Guid.Empty)
            throw new DomainException("ImigranteId é obrigatório.");

        ValidarAntesDeArmazenar(NomeOriginal, TamanhoBytes);

        if (string.IsNullOrWhiteSpace(NomeArmazenado))
            throw new DomainException("Nome de armazenamento do arquivo não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(CaminhoRelativo))
            throw new DomainException("Caminho do arquivo não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(ContentType))
            throw new DomainException("Content-Type do arquivo é obrigatório.");

        if (HashSha256.Length != 64 || !HashSha256.All(Uri.IsHexDigit))
            throw new DomainException("Hash do arquivo deve ser um SHA-256 em hexadecimal.");

        if (!Enum.IsDefined(typeof(TipoArquivoImigranteEnum), TipoArquivo))
            throw new DomainException("Tipo de arquivo inválido.");
    }
}
