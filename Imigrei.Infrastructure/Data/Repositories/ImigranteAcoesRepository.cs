using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Interfaces;
using Imigrei.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Imigrei.Infra.Repositories;

public class ImigranteAcoesRepository : IImigranteAcoesRepository
{
    private readonly AppDbContext _context;

    public ImigranteAcoesRepository(AppDbContext context)
    {
        _context = context;
    }

    // Rastreado: o serviço muta a instância retornada na edição.
    public async Task<ImigranteAcoes?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ImigranteAcoes
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<ImigranteAcoes>> GetByImigranteIdAsync(
        Guid imigranteId, CancellationToken cancellationToken = default)
        => await _context.ImigranteAcoes
            .AsNoTracking()
            .Where(a => a.ImigranteId == imigranteId)
            .OrderByDescending(a => a.DataRegistro)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ImigranteAcoes acao, CancellationToken cancellationToken = default)
        => await _context.ImigranteAcoes.AddAsync(acao, cancellationToken);

    public void Delete(ImigranteAcoes acao)
        => _context.ImigranteAcoes.Remove(acao);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
