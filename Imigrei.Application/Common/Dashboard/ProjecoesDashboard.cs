using Imigrei.Domain.Enums;

namespace Imigrei.Application.Common.Dashboard;

/// <summary>Quantos imigrantes há por país de origem.</summary>
public sealed record ImigrantesPorNacionalidade(string PaisOrigem, int Total);

/// <summary>
/// Uma ação do histórico já cruzada com o nome do imigrante — a junção acontece
/// no banco, não em memória.
/// </summary>
public sealed record AtividadeRecente(
    Guid AcaoId,
    Guid ImigranteId,
    string NomeImigrante,
    DateTime DataRegistro,
    string Acao,
    TipoAcaoImigranteEnum TipoAcao);
