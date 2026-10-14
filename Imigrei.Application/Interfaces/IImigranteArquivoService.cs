using Imigrei.Application.DTOs;
using Imigrei.Domain.Enums;

namespace Imigrei.Application.Interfaces;

public interface IImigranteArquivoService
{
    Task<IEnumerable<ImigranteArquivoResponseDto>> GetByImigranteIdAsync(
        Guid imigranteId, CancellationToken cancellationToken = default);

    /// <param name="tipoArquivo">
    /// Classificação do documento. Opcional: sem ela o arquivo entra como
    /// <see cref="TipoArquivoImigranteEnum.Outro"/>.
    /// </param>
    Task<ImigranteArquivoResponseDto> UploadAsync(
        Guid imigranteId, Stream conteudo, string nomeOriginal, string contentType,
        long tamanhoBytes, TipoArquivoImigranteEnum tipoArquivo = TipoArquivoImigranteEnum.Outro,
        CancellationToken cancellationToken = default);

    Task<(Stream Conteudo, string ContentType, string NomeOriginal)> DownloadAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
