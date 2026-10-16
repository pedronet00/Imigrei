using ClosedXML.Excel;
using Imigrei.Application.Common.Relatorios;
using Imigrei.Application.Interfaces;

namespace Imigrei.Infra.Relatorios;

/// <summary>
/// Planilha .xlsx com título, período e resumo no topo e os dados como tabela
/// do Excel (filtro e ordenação prontos). Números e datas vão como tipos
/// nativos, para que o usuário consiga somar e filtrar.
/// </summary>
public class RelatorioExcelExportador : IRelatorioExportador
{
    private const string ContentTypeXlsx =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public FormatoRelatorio Formato => FormatoRelatorio.Excel;

    public ArquivoRelatorio Exportar(Relatorio relatorio)
    {
        using var pasta = new XLWorkbook();
        var planilha = pasta.Worksheets.Add(NomeDaAba(relatorio.Titulo));

        var linha = 1;
        planilha.Cell(linha, 1).Value = relatorio.Titulo;
        planilha.Cell(linha, 1).Style.Font.SetBold().Font.SetFontSize(16);
        linha++;

        planilha.Cell(linha, 1).Value = $"Período: {relatorio.Periodo.Descricao}";
        linha++;

        planilha.Cell(linha, 1).Value =
            $"Emitido em {HorarioBrasilia.DeUtc(DateTime.UtcNow):dd/MM/yyyy HH:mm}";
        planilha.Cell(linha, 1).Style.Font.SetFontColor(XLColor.Gray);
        linha += 2;

        foreach (var item in relatorio.Resumo)
        {
            planilha.Cell(linha, 1).Value = item.Rotulo;
            planilha.Cell(linha, 2).Value = item.Valor;
            planilha.Cell(linha, 2).Style.Font.SetBold();
            linha++;
        }
        linha++;

        var inicioTabela = linha;
        for (var c = 0; c < relatorio.Colunas.Count; c++)
            planilha.Cell(linha, c + 1).Value = relatorio.Colunas[c].Titulo;

        foreach (var registro in relatorio.Linhas)
        {
            linha++;
            for (var c = 0; c < relatorio.Colunas.Count; c++)
                PreencherCelula(planilha.Cell(linha, c + 1), registro[c], relatorio.Colunas[c].Tipo);
        }

        // Uma tabela precisa de ao menos uma linha de dados.
        if (relatorio.Linhas.Count == 0)
        {
            linha++;
            planilha.Cell(linha, 1).Value = "Nenhum registro encontrado no período.";
        }

        var tabela = planilha.Range(inicioTabela, 1, linha, relatorio.Colunas.Count).CreateTable();
        tabela.Theme = XLTableTheme.TableStyleMedium2;

        planilha.Columns(1, relatorio.Colunas.Count).AdjustToContents(inicioTabela, linha, 8, 60);
        planilha.SheetView.FreezeRows(inicioTabela);

        using var saida = new MemoryStream();
        pasta.SaveAs(saida);

        return new ArquivoRelatorio(
            saida.ToArray(),
            ContentTypeXlsx,
            $"{relatorio.NomeArquivo}_{relatorio.Periodo.SufixoArquivo}.xlsx");
    }

    private static void PreencherCelula(IXLCell celula, object? valor, TipoColunaRelatorio tipo)
    {
        switch (valor)
        {
            case null:
                return;
            case DateTime data:
                celula.Value = data;
                celula.Style.DateFormat.Format =
                    tipo == TipoColunaRelatorio.Data ? "dd/mm/yyyy" : "dd/mm/yyyy hh:mm";
                return;
            case int or long or decimal or double:
                celula.Value = Convert.ToDouble(valor);
                celula.Style.NumberFormat.Format =
                    tipo == TipoColunaRelatorio.Percentual ? "0.0%" : "#,##0";
                return;
            default:
                // Texto explícito: evita que telefone ou CEP virem número.
                celula.Value = valor.ToString();
                return;
        }
    }

    /// <summary>Abas do Excel: no máximo 31 caracteres e sem <c>: \ / ? * [ ]</c>.</summary>
    private static string NomeDaAba(string titulo)
    {
        var limpo = new string(titulo.Where(ch => ":\\/?*[]".IndexOf(ch) < 0).ToArray());
        return limpo.Length <= 31 ? limpo : limpo[..31];
    }
}
