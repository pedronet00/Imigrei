using Microsoft.AspNetCore.Identity;

namespace Imigrei.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }

    public ApplicationRole(string nome) : base(nome) { }
}
