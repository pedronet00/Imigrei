namespace Imigrei.Domain.Constants;

/// <summary>
/// Perfis de acesso reconhecidos pelo domínio. São constantes — e não um enum —
/// porque viajam como claims de autorização (strings) até a borda da aplicação.
/// </summary>
public static class PerfisUsuario
{
    public const string Administrador = "Administrador";
    public const string Atendente = "Atendente";
    public const string Imigrante = "Imigrante";

    public static readonly IReadOnlyList<string> Todos =
        new[] { Administrador, Atendente, Imigrante };

    public static bool EhValido(string? perfil) =>
        perfil is not null && Todos.Contains(perfil, StringComparer.OrdinalIgnoreCase);
}
