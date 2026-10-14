using Imigrei.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Imigrei.API.Authentication;

/// <summary>
/// Lê o usuário da requisição em curso a partir das claims do JWT.
/// Fica na borda porque é o único ponto que conhece o <see cref="HttpContext"/>.
/// </summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    public Guid? Id =>
        Guid.TryParse(
            Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub),
            out var id)
            ? id
            : null;

    public string? Email =>
        Principal?.FindFirstValue(ClaimTypes.Email)
        ?? Principal?.FindFirstValue(JwtRegisteredClaimNames.Email);

    public bool EstaAutenticado => Principal?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<string> Perfis =>
        Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

    public string? EnderecoIp => _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
