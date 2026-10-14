using Imigrei.Application.Common.Dashboard;
using Imigrei.Domain.Enums;

namespace Imigrei.Application.DTOs.Dashboard;

public record DashboardResponseDto(
    int TotalImigrantes,
    int NovosEsteMes,
    int TotalNacionalidades,
    int TotalDocumentos,
    IReadOnlyList<ImigrantesPorNacionalidadeDto> ImigrantesPorNacionalidade,
    IReadOnlyList<AtividadeRecenteDto> AtividadeRecente);

public record ImigrantesPorNacionalidadeDto(string PaisOrigem, int Total)
{
    public static ImigrantesPorNacionalidadeDto FromProjecao(ImigrantesPorNacionalidade p)
        => new(p.PaisOrigem, p.Total);
}

public record AtividadeRecenteDto(
    Guid AcaoId,
    Guid ImigranteId,
    string NomeImigrante,
    DateTime DataRegistro,
    string Acao,
    TipoAcaoImigranteEnum TipoAcao)
{
    public static AtividadeRecenteDto FromProjecao(AtividadeRecente a)
        => new(a.AcaoId, a.ImigranteId, a.NomeImigrante, a.DataRegistro, a.Acao, a.TipoAcao);
}
