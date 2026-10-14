// Imigrei.Infra/Repositories/ImigranteRepository.cs
using Imigrei.Domain.Entities;
using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Interfaces;
using Imigrei.Infra.Data;
using Imigrei.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Imigrei.Infra.Repositories;

public class ImigranteRepository : IImigranteRepository
{
    private readonly AppDbContext _context;

    public ImigranteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Imigrante?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Imigrantes
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IEnumerable<Imigrante>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Imigrantes
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Imigrante imigrante, CancellationToken cancellationToken = default)
        => await _context.Imigrantes.AddAsync(imigrante, cancellationToken);

    public void Update(Imigrante imigrante)
        => _context.Imigrantes.Update(imigrante);

    public void Delete(Imigrante imigrante)
        => _context.Imigrantes.Remove(imigrante);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Imigrantes
            .AsNoTracking()
            .AnyAsync(i => i.Email == email, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}