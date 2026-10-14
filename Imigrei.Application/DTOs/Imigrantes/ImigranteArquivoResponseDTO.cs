using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Enums;

namespace Imigrei.Application.DTOs;

/// <param name="HashSha256">
/// Permite ao cliente conferir por conta própria o arquivo que baixou.
/// </param>
public record ImigranteArquivoResponseDto(
    Guid Id,
    Guid ImigranteId,
    string NomeOriginal,
    string ContentType,
    long TamanhoBytes,
    string HashSha256,
    TipoArquivoImigranteEnum TipoArquivo,
    DateTime DataUpload)
{
    public static ImigranteArquivoResponseDto FromEntity(ImigranteArquivo entity)
        => new(entity.Id, entity.ImigranteId, entity.NomeOriginal, entity.ContentType,
               entity.TamanhoBytes, entity.HashSha256, entity.TipoArquivo, entity.DataUpload);
}
