using System.Globalization;
using Imigrei.Application.Common.Relatorios;

namespace Imigrei.Infra.Relatorios;

/// <summary>Texto pt-BR de uma célula, para formatos sem tipos nativos (PDF).</summary>
internal static class FormatacaoCelula
{
    public static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static string Texto(object? valor, TipoColunaRelatorio tipo) => (valor, tipo) switch
    {
        (null, _) => string.Empty,
        (DateTime d, TipoColunaRelatorio.Data) => d.ToString("dd/MM/yyyy", PtBr),
        (DateTime d, _) => d.ToString("dd/MM/yyyy HH:mm", PtBr),
        (IFormattable n, TipoColunaRelatorio.Percentual) => n.ToString("P1", PtBr),
        (IFormattable n, TipoColunaRelatorio.Inteiro) => n.ToString("N0", PtBr),
        _ => Convert.ToString(valor, PtBr) ?? string.Empty,
    };

    public static bool AlinharADireita(TipoColunaRelatorio tipo)
        => tipo is TipoColunaRelatorio.Inteiro or TipoColunaRelatorio.Percentual;
}
