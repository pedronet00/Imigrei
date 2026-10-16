using Imigrei.Application.Common.Relatorios;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Enums;
using Imigrei.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Imigrei.Infra.Data.Queries;

/// <summary>
/// Consultas dos relatórios. Projeta só as colunas exibidas, sem rastreamento
/// e sem materializar entidades.
/// </summary>
public class RelatoriosQuery : IRelatoriosQuery
{
    private readonly AppDbContext _context;

    public RelatoriosQuery(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ImigranteRelatorio>> ObterImigrantesCadastradosAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc, CancellationToken cancellationToken = default)
        => await ProjetarImigrantes(ImigrantesCadastradosEntre(inicioUtc, fimExclusivoUtc))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<NacionalidadeRelatorio>> ObterImigrantesPorNacionalidadeAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc, CancellationToken cancellationToken = default)
    {
        // Tipo anônimo no GROUP BY, mapeado depois — mesmo motivo do dashboard.
        var linhas = await ImigrantesCadastradosEntre(inicioUtc, fimExclusivoUtc)
            .GroupBy(i => i.PaisOrigem)
            .Select(g => new { PaisOrigem = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.PaisOrigem)
            .ToListAsync(cancellationToken);

        return linhas.Select(x => new NacionalidadeRelatorio(x.PaisOrigem, x.Total)).ToList();
    }

    public async Task<IReadOnlyList<AcaoRelatorio>> ObterAcoesAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc,
        IReadOnlyCollection<TipoAcaoImigranteEnum> tipos,
        CancellationToken cancellationToken = default)
    {
        var acoes = _context.ImigranteAcoes.AsNoTracking();

        if (inicioUtc is not null)
            acoes = acoes.Where(a => a.DataRegistro >= inicioUtc);
        if (fimExclusivoUtc is not null)
            acoes = acoes.Where(a => a.DataRegistro < fimExclusivoUtc);
        if (tipos.Count > 0)
        {
            var filtro = tipos.ToArray();
            acoes = acoes.Where(a => filtro.Contains(a.TipoAcao));
        }

        return await acoes
            .OrderBy(a => a.DataRegistro)
            .Select(a => new AcaoRelatorio(
                a.Id,
                a.ImigranteId,
                a.Imigrante.Nome,
                a.Imigrante.PaisOrigem,
                a.DataRegistro,
                a.Acao,
                a.TipoAcao))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentoRelatorio>> ObterDocumentosEnviadosAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc, CancellationToken cancellationToken = default)
    {
        var arquivos = _context.ImigranteArquivos.AsNoTracking();

        if (inicioUtc is not null)
            arquivos = arquivos.Where(a => a.DataUpload >= inicioUtc);
        if (fimExclusivoUtc is not null)
            arquivos = arquivos.Where(a => a.DataUpload < fimExclusivoUtc);

        return await arquivos
            .OrderBy(a => a.DataUpload)
            .Select(a => new DocumentoRelatorio(
                a.Imigrante.Nome,
                a.TipoArquivo,
                a.NomeOriginal,
                a.TamanhoBytes,
                a.DataUpload))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ImigranteRelatorio>> ObterImigrantesComStatusCpfAsync(
        DateTime? inicioUtc, DateTime? fimExclusivoUtc,
        IReadOnlyCollection<StatusEmissaoCPFEnum> status,
        CancellationToken cancellationToken = default)
    {
        var filtro = status.ToArray();

        return await ProjetarImigrantes(ImigrantesCadastradosEntre(inicioUtc, fimExclusivoUtc)
                .Where(i => filtro.Contains(i.StatusEmissaoCPF)))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Imigrante> ImigrantesCadastradosEntre(DateTime? inicioUtc, DateTime? fimExclusivoUtc)
    {
        var imigrantes = _context.Imigrantes.AsNoTracking();

        if (inicioUtc is not null)
            imigrantes = imigrantes.Where(i => i.DataCadastro >= inicioUtc);
        if (fimExclusivoUtc is not null)
            imigrantes = imigrantes.Where(i => i.DataCadastro < fimExclusivoUtc);

        return imigrantes;
    }

    private static IQueryable<ImigranteRelatorio> ProjetarImigrantes(IQueryable<Imigrante> imigrantes)
        => imigrantes
            .OrderBy(i => i.DataCadastro)
            .ThenBy(i => i.Nome)
            .Select(i => new ImigranteRelatorio(
                i.Id,
                i.Nome,
                i.PaisOrigem,
                i.Email,
                i.Telefone,
                i.Cidade,
                i.Uf,
                i.StatusEmissaoCPF,
                i.DataCadastro));
}
