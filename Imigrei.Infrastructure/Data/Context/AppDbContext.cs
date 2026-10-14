using Imigrei.Domain.Entities.Identidade;
using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Imigrei.Infrastructure.Data.Context;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Imigrante> Imigrantes => Set<Imigrante>();
    public DbSet<ImigranteArquivo> ImigranteArquivos => Set<ImigranteArquivo>();
    public DbSet<ImigranteAcoes> ImigranteAcoes => Set<ImigranteAcoes>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // As tabelas do Identity são criadas pela base; as configurações do
        // assembly renomeiam-nas e ajustam os tipos logo em seguida.
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
