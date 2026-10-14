using Imigrei.Domain.Entities.Imigrantes;

namespace Imigrei.Domain.Interfaces;

public interface IImigranteRepository
{
    Task<Imigrante?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Imigrante>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Imigrante imigrante, CancellationToken cancellationToken = default);
    void Update(Imigrante imigrante);
    void Delete(Imigrante imigrante);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}