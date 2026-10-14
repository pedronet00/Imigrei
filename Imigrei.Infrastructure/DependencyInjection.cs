using Imigrei.Application.Interfaces;
using Imigrei.Domain.Interfaces;
using Imigrei.Infra.Data.Queries;
using Imigrei.Infra.Repositories;
using Imigrei.Infra.Storage;
using Imigrei.Infrastructure.Authentication;
using Imigrei.Infrastructure.Data.Context;
using Imigrei.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imigrei.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra persistência, armazenamento de arquivos e o provedor de identidade.
    /// A configuração do esquema de autenticação (JWT Bearer) fica na borda (API).
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Connection string 'MySql' não configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<IImigranteRepository, ImigranteRepository>();
        services.AddScoped<IImigranteArquivoRepository, ImigranteArquivoRepository>();
        services.AddScoped<IImigranteAcoesRepository, ImigranteAcoesRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IDashboardQuery, DashboardQuery>();
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
        services.AddScoped<IArquivoStorage, ArquivoStorageLocal>();

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
