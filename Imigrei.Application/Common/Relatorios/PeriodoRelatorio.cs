using Imigrei.Domain.Exceptions;

namespace Imigrei.Application.Common.Relatorios;

/// <summary>
/// Intervalo de datas pedido pelo usuário, com as duas pontas inclusivas e
/// opcionais. As datas são dias do calendário de Brasília; os limites em UTC
/// (a referência de gravação no banco) saem daqui, com o fim exclusivo.
/// </summary>
public sealed class PeriodoRelatorio
{
    public DateOnly? Inicio { get; }
    public DateOnly? Fim { get; }

    /// <summary>Primeiro instante do dia inicial, em UTC.</summary>
    public DateTime? InicioUtc { get; }

    /// <summary>Primeiro instante do dia seguinte ao final, em UTC — usar com <c>&lt;</c>.</summary>
    public DateTime? FimExclusivoUtc { get; }

    private PeriodoRelatorio(DateOnly? inicio, DateOnly? fim)
    {
        Inicio = inicio;
        Fim = fim;
        InicioUtc = inicio is { } i ? HorarioBrasilia.InicioDoDiaEmUtc(i) : null;
        FimExclusivoUtc = fim is { } f ? HorarioBrasilia.InicioDoDiaEmUtc(f.AddDays(1)) : null;
    }

    public static PeriodoRelatorio Criar(DateOnly? inicio, DateOnly? fim)
    {
        if (inicio is not null && fim is not null && inicio > fim)
            throw new DomainException("A data inicial não pode ser posterior à data final.");

        return new PeriodoRelatorio(inicio, fim);
    }

    public string Descricao => (Inicio, Fim) switch
    {
        (null, null) => "Todo o período",
        ({ } i, null) => $"A partir de {i:dd/MM/yyyy}",
        (null, { } f) => $"Até {f:dd/MM/yyyy}",
        ({ } i, { } f) => $"{i:dd/MM/yyyy} a {f:dd/MM/yyyy}",
    };

    /// <summary>Trecho do nome do arquivo, ex.: <c>2026-10-12_a_2026-10-20</c>.</summary>
    public string SufixoArquivo => (Inicio, Fim) switch
    {
        (null, null) => "completo",
        ({ } i, null) => $"desde_{i:yyyy-MM-dd}",
        (null, { } f) => $"ate_{f:yyyy-MM-dd}",
        ({ } i, { } f) => $"{i:yyyy-MM-dd}_a_{f:yyyy-MM-dd}",
    };
}
