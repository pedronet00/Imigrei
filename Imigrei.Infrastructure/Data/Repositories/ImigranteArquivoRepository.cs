// Imigrei.Infra/Repositories/ImigranteArquivoRepository.cs
using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Interfaces;
using Imigrei.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Imigrei.Infra.Repositories;

public class ImigranteArquivoRepository : IImigranteArquivoRepository
{
    private readonly AppDbContext _context;

    public ImigranteArquivoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ImigranteArquivo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ImigranteArquivos
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<ImigranteArquivo>> GetByImigranteIdAsync(Guid imigranteId, CancellationToken cancellationToken = default)
        => await _context.ImigranteArquivos
            .AsNoTracking()
            .Where(a => a.ImigranteId == imigranteId)
            .OrderByDescending(a => a.DataUpload)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ImigranteArquivo arquivo, CancellationToken cancellationToken = default)
        => await _context.ImigranteArquivos.AddAsync(arquivo, cancellationToken);

    public void Delete(ImigranteArquivo arquivo)
        => _context.ImigranteArquivos.Remove(arquivo);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}