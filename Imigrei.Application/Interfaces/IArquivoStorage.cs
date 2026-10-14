using Imigrei.Application.Common.Storage;

namespace Imigrei.Application.Interfaces;

/// <summary>
/// Porta de armazenamento de conteúdo. A implementação atual grava em disco
/// (local ou volume da VPS); trocar por S3/Blob não afeta domínio nem aplicação.
/// </summary>
public interface IArquivoStorage
{
    /// <summary>
    /// Grava o stream sob um nome opaco, calculando tamanho e SHA-256 durante a escrita.
    /// </summary>
    /// <param name="tamanhoMaximoBytes">
    /// Corta a gravação se o conteúdo passar disso, mesmo que o cliente tenha
    /// declarado um tamanho menor.
    /// </param>
    Task<ArquivoArmazenado> SalvarAsync(
        Stream conteudo, string nomeOriginal, Guid imigranteId,
        long tamanhoMaximoBytes, CancellationToken cancellationToken = default);

    Task<Stream> AbrirLeituraAsync(string caminhoRelativo, CancellationToken cancellationToken = default);

    void Excluir(string caminhoRelativo);
}
