using Imigrei.Domain.Constants;
using Imigrei.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Imigrei.Infrastructure.Data.Seed;

/// <summary>
/// Garante que os perfis de acesso existam no banco. Idempotente: pode rodar a
/// cada inicialização.
/// </summary>
/// <remarks>
/// Só perfis — nenhum usuário. Credenciais nunca vêm de configuração: todo
/// usuário nasce no banco, pelo cadastro ou pela mão do administrador.
/// </remarks>
public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var escopo = services.CreateScope();

        var roleManager = escopo.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var perfil in PerfisUsuario.Todos)
        {
            if (!await roleManager.RoleExistsAsync(perfil))
                await roleManager.CreateAsync(new ApplicationRole(perfil));
        }
    }
}
