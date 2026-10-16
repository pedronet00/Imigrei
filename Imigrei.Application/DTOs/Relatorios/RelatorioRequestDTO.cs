using Imigrei.Application.Common.Relatorios;

namespace Imigrei.Application.DTOs.Relatorios;

/// <summary>
/// Filtros comuns a todos os relatórios, vindos da query string:
/// <c>?formato=pdf&amp;dataInicio=2026-10-12&amp;dataFim=2026-10-20</c>.
/// O formato aceita <c>pdf</c> ou <c>excel</c>, sem diferenciar maiúsculas.
/// </summary>
public class RelatorioRequestDto
{
    public FormatoRelatorio Formato { get; init; } = FormatoRelatorio.Pdf;

    public DateOnly? DataInicio { get; init; }

    public DateOnly? DataFim { get; init; }
}
