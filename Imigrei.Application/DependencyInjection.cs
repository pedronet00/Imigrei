using Imigrei.Application.Interfaces;
using Imigrei.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Imigrei.Application;

public static class DependencyInjection
{
    /// <summary>Registra os casos de uso da camada de aplicação.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IImigranteService, ImigranteService>();
        services.AddScoped<IImigranteArquivoService, ImigranteArquivoService>();
        services.AddScoped<IImigranteAcoesService, ImigranteAcoesService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
