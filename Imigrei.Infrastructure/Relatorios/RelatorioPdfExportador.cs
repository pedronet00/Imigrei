using Imigrei.Application.Common.Relatorios;
using Imigrei.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Imigrei.Infra.Relatorios;

/// <summary>
/// PDF em A4 paisagem: cabeçalho com título e período, resumo, tabela com
/// cabeçalho repetido a cada página e rodapé com data de emissão e paginação.
/// </summary>
public class RelatorioPdfExportador : IRelatorioExportador
{
    private const string CorDestaque = "#1F4E79";

    static RelatorioPdfExportador()
    {
        // Licença Community: gratuita para organizações com receita anual
        // abaixo de US$ 1 milhão. Acima disso é preciso uma licença paga.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public FormatoRelatorio Formato => FormatoRelatorio.Pdf;

    public ArquivoRelatorio Exportar(Relatorio relatorio)
    {
        var emitidoEm = HorarioBrasilia.DeUtc(DateTime.UtcNow);

        var conteudo = Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4.Landscape());
                pagina.Margin(1.5f, Unit.Centimetre);
                pagina.DefaultTextStyle(t => t.FontSize(9));

                pagina.Header().Column(cabecalho =>
                {
                    cabecalho.Item().Text("Imigrei").FontSize(10).FontColor(Colors.Grey.Darken1);
                    cabecalho.Item().Text(relatorio.Titulo).FontSize(18).Bold().FontColor(CorDestaque);
                    cabecalho.Item().Text($"Período: {relatorio.Periodo.Descricao}").FontSize(10);
                    cabecalho.Item().PaddingTop(6).LineHorizontal(1).LineColor(CorDestaque);
                });

                pagina.Content().PaddingTop(10).Column(corpo =>
                {
                    corpo.Spacing(12);
                    corpo.Item().Element(c => Resumo(c, relatorio.Resumo));

                    if (relatorio.Linhas.Count == 0)
                        corpo.Item().PaddingTop(20).AlignCenter()
                            .Text("Nenhum registro encontrado no período.").Italic().FontColor(Colors.Grey.Darken1);
                    else
                        corpo.Item().Element(c => Tabela(c, relatorio));
                });

                pagina.Footer().Row(rodape =>
                {
                    rodape.RelativeItem().Text($"Emitido em {emitidoEm:dd/MM/yyyy HH:mm}")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                    rodape.RelativeItem().AlignRight().Text(t =>
                    {
                        t.DefaultTextStyle(s => s.FontSize(8).FontColor(Colors.Grey.Darken1));
                        t.Span("Página ");
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
                });
            });
        }).GeneratePdf();

        return new ArquivoRelatorio(
            conteudo,
            "application/pdf",
            $"{relatorio.NomeArquivo}_{relatorio.Periodo.SufixoArquivo}.pdf");
    }

    private static void Resumo(IContainer container, IReadOnlyList<ItemResumoRelatorio> itens)
    {
        container.Background(Colors.Grey.Lighten4).Padding(8).Column(coluna =>
        {
            coluna.Spacing(2);
            foreach (var item in itens)
                coluna.Item().Text(t =>
                {
                    t.Span($"{item.Rotulo}: ");
                    t.Span(item.Valor).Bold();
                });
        });
    }

    private static void Tabela(IContainer container, Relatorio relatorio)
    {
        container.Table(tabela =>
        {
            tabela.ColumnsDefinition(colunas =>
            {
                foreach (var coluna in relatorio.Colunas)
                    colunas.RelativeColumn(coluna.Peso);
            });

            tabela.Header(cabecalho =>
            {
                foreach (var coluna in relatorio.Colunas)
                {
                    var celula = cabecalho.Cell().Background(CorDestaque).Padding(4);
                    (FormatacaoCelula.AlinharADireita(coluna.Tipo) ? celula.AlignRight() : celula)
                        .Text(coluna.Titulo).FontColor(Colors.White).Bold();
                }
            });

            for (var i = 0; i < relatorio.Linhas.Count; i++)
            {
                var linha = relatorio.Linhas[i];
                var fundo = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;

                for (var c = 0; c < relatorio.Colunas.Count; c++)
                {
                    var tipo = relatorio.Colunas[c].Tipo;
                    var celula = tabela.Cell()
                        .Background(fundo)
                        .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                        .Padding(4);

                    (FormatacaoCelula.AlinharADireita(tipo) ? celula.AlignRight() : celula)
                        .Text(FormatacaoCelula.Texto(linha[c], tipo));
                }
            }
        });
    }
}
