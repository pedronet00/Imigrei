namespace Imigrei.Application.Common.Relatorios;

public enum FormatoRelatorio
{
    Pdf,
    Excel,
}

/// <summary>
/// Diz ao exportador como apresentar a célula: no Excel vira o tipo nativo
/// (número, data), no PDF vira o texto formatado em pt-BR.
/// </summary>
public enum TipoColunaRelatorio
{
    Texto,
    Inteiro,
    Data,
    DataHora,
    Percentual,
}

/// <param name="Peso">Largura relativa da coluna no PDF.</param>
public sealed record ColunaRelatorio(
    string Titulo,
    TipoColunaRelatorio Tipo = TipoColunaRelatorio.Texto,
    float Peso = 1);

/// <summary>Par rótulo/valor exibido acima da tabela (totais, contagens).</summary>
public sealed record ItemResumoRelatorio(string Rotulo, string Valor);

/// <summary>
/// Relatório já montado e independente de formato: os casos de uso decidem o
/// conteúdo, os exportadores só decidem a aparência. Datas nas linhas já vêm
/// no horário de Brasília.
/// </summary>
public sealed record Relatorio(
    string Titulo,
    string NomeArquivo,
    PeriodoRelatorio Periodo,
    IReadOnlyList<ColunaRelatorio> Colunas,
    IReadOnlyList<object?[]> Linhas,
    IReadOnlyList<ItemResumoRelatorio> Resumo);

/// <summary>Arquivo pronto para ser devolvido ao cliente.</summary>
public sealed record ArquivoRelatorio(byte[] Conteudo, string ContentType, string NomeArquivo);
