using Imigrei.Domain.Entities.Identidade;
using Imigrei.Domain.Interfaces;
using Imigrei.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Imigrei.Infra.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca pelo hash do valor bruto — o token em claro nunca chega ao banco.
    /// </summary>
    public async Task<RefreshToken?> GetByTokenAsync(string tokenBruto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tokenBruto))
            return null;

        var hash = RefreshToken.GerarHash(tokenBruto);

        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetAtivosPorUsuarioAsync(
        Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var agora = DateTime.UtcNow;

        return await _context.RefreshTokens
            .Where(t => t.UsuarioId == usuarioId && t.RevogadoEm == null && t.ExpiraEm > agora)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    public void Update(RefreshToken refreshToken)
        => _context.RefreshTokens.Update(refreshToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
