using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Enums;

namespace Imigrei.Application.DTOs;

public record ImigranteAcaoResponseDto(
    Guid Id,
    Guid ImigranteId,
    string Acao,
    TipoAcaoImigranteEnum TipoAcao,
    DateTime DataRegistro)
{
    public static ImigranteAcaoResponseDto FromEntity(ImigranteAcoes a)
        => new(a.Id, a.ImigranteId, a.Acao, a.TipoAcao, a.DataRegistro);
}
