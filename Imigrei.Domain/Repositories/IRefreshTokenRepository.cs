using Imigrei.Domain.Entities.Identidade;

namespace Imigrei.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string tokenBruto, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshToken>> GetAtivosPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    void Update(RefreshToken refreshToken);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
