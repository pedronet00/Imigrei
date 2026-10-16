using Imigrei.Application.Common.Relatorios;

namespace Imigrei.Application.Interfaces;

/// <summary>
/// Transforma um <see cref="Relatorio"/> em arquivo. Há uma implementação por
/// formato; a biblioteca de cada um fica restrita à infraestrutura.
/// </summary>
public interface IRelatorioExportador
{
    FormatoRelatorio Formato { get; }

    ArquivoRelatorio Exportar(Relatorio relatorio);
}
