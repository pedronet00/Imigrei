using Imigrei.Application.Common.Dashboard;

namespace Imigrei.Application.Interfaces;

/// <summary>
/// Porta de leitura do dashboard. É um modelo de consulta, não um agregado:
/// cada método vira uma agregação no banco, sem carregar entidades em memória
/// só para contá-las. Por isso vive aqui, e não entre os repositórios do domínio.
/// </summary>
public interface IDashboardQuery
{
    Task<int> ContarImigrantesAsync(CancellationToken cancellationToken = default);

    /// <summary>Imigrantes cadastrados a partir de <paramref name="inicio"/> (UTC).</summary>
    Task<int> ContarImigrantesCadastradosDesdeAsync(DateTime inicio, CancellationToken cancellationToken = default);

    /// <summary>Quantos países de origem distintos aparecem na base.</summary>
    Task<int> ContarNacionalidadesAsync(CancellationToken cancellationToken = default);

    Task<int> ContarDocumentosAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ImigrantesPorNacionalidade>> ObterImigrantesPorNacionalidadeAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AtividadeRecente>> ObterAtividadesRecentesAsync(
        int quantidade, CancellationToken cancellationToken = default);
}
