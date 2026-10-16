using Imigrei.Application.Common.Relatorios;
using Imigrei.Domain.Enums;

namespace Imigrei.Application.Interfaces;

/// <summary>
/// Porta de leitura dos relatórios. Como no dashboard, é modelo de consulta:
/// projeções e agregações rodam no banco. Os limites são instantes UTC,
/// inicial inclusivo e final exclusivo; nulo significa "sem limite".
/// </summary>
public interface IRelatoriosQuery
{
    Task<IReadOnlyList<ImigranteRelatorio>> ObterImigrantesCadastradosAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NacionalidadeRelatorio>> ObterImigrantesPorNacionalidadeAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc, CancellationToken cancellationToken = default);

    /// <summary>Ações do histórico; <paramref name="tipos"/> vazio traz todos os tipos.</summary>
    Task<IReadOnlyList<AcaoRelatorio>> ObterAcoesAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc,
        IReadOnlyCollection<TipoAcaoImigranteEnum> tipos,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DocumentoRelatorio>> ObterDocumentosEnviadosAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ImigranteRelatorio>> ObterImigrantesComStatusCpfAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc,
        IReadOnlyCollection<StatusEmissaoCPFEnum> status,
        CancellationToken cancellationToken = default);
}
