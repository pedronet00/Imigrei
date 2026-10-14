using Microsoft.AspNetCore.Identity;

namespace Imigrei.Infrastructure.Identity;

/// <summary>
/// Modelo de persistência do usuário, próprio do ASP.NET Core Identity.
/// Vive na infraestrutura: o domínio e a aplicação o enxergam apenas através
/// da projeção <c>UsuarioIdentidade</c>.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string Nome { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
