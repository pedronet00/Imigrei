using Imigrei.Application.Common.Dashboard;
using Imigrei.Application.Interfaces;
using Imigrei.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Imigrei.Infra.Data.Queries;

/// <summary>
/// Agregações do dashboard. Tudo roda como COUNT/GROUP BY/JOIN no banco —
/// nenhuma entidade é materializada só para ser contada.
/// </summary>
public class DashboardQuery : IDashboardQuery
{
    private readonly AppDbContext _context;

    public DashboardQuery(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> ContarImigrantesAsync(CancellationToken cancellationToken = default)
        => await _context.Imigrantes.CountAsync(cancellationToken);

    public async Task<int> ContarImigrantesCadastradosDesdeAsync(
        DateTime inicio, CancellationToken cancellationToken = default)
        => await _context.Imigrantes
            .Where(i => i.DataCadastro >= inicio)
            .CountAsync(cancellationToken);

    public async Task<int> ContarNacionalidadesAsync(CancellationToken cancellationToken = default)
        => await _context.Imigrantes
            .Select(i => i.PaisOrigem)
            .Distinct()
            .CountAsync(cancellationToken);

    public async Task<int> ContarDocumentosAsync(CancellationToken cancellationToken = default)
        => await _context.ImigranteArquivos.CountAsync(cancellationToken);

    public async Task<IReadOnlyList<ImigrantesPorNacionalidade>> ObterImigrantesPorNacionalidadeAsync(
        CancellationToken cancellationToken = default)
    {
        // Tipo anônimo no agrupamento: o tradutor de consultas não constrói um
        // record com construtor dentro de um GROUP BY. O mapeamento fica depois,
        // sobre a lista já agregada pelo banco.
        var linhas = await _context.Imigrantes
            .GroupBy(i => i.PaisOrigem)
            .Select(g => new { PaisOrigem = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.PaisOrigem)
            .ToListAsync(cancellationToken);

        return linhas
            .Select(x => new ImigrantesPorNacionalidade(x.PaisOrigem, x.Total))
            .ToList();
    }

    public async Task<IReadOnlyList<AtividadeRecente>> ObterAtividadesRecentesAsync(
        int quantidade, CancellationToken cancellationToken = default)
        => await _context.ImigranteAcoes
            .OrderByDescending(a => a.DataRegistro)
            .Take(quantidade)
            // O nome do imigrante sai da junção, na mesma consulta.
            .Select(a => new AtividadeRecente(
                a.Id,
                a.ImigranteId,
                a.Imigrante.Nome,
                a.DataRegistro,
                a.Acao,
                a.TipoAcao))
            .ToListAsync(cancellationToken);
}
