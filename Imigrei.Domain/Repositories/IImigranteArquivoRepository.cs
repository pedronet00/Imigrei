using Imigrei.Domain.Entities.Imigrantes;

namespace Imigrei.Domain.Interfaces;

public interface IImigranteArquivoRepository
{
    Task<ImigranteArquivo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ImigranteArquivo>> GetByImigranteIdAsync(Guid imigranteId, CancellationToken cancellationToken = default);
    Task AddAsync(ImigranteArquivo arquivo, CancellationToken cancellationToken = default);
    void Delete(ImigranteArquivo arquivo);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}