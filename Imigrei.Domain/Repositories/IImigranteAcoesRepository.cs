using Imigrei.Domain.Entities.Imigrantes;

namespace Imigrei.Domain.Interfaces;

public interface IImigranteAcoesRepository
{
    Task<ImigranteAcoes?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ImigranteAcoes>> GetByImigranteIdAsync(Guid imigranteId, CancellationToken cancellationToken = default);
    Task AddAsync(ImigranteAcoes acao, CancellationToken cancellationToken = default);
    void Delete(ImigranteAcoes acao);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
