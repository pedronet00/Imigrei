namespace Imigrei.Application.Common.Identidade;

/// <summary>
/// Projeção do usuário vinda do provedor de identidade. Existe para que a camada
/// de aplicação não conheça os tipos do ASP.NET Core Identity.
/// </summary>
public sealed record UsuarioIdentidade(
    Guid Id,
    string Nome,
    string Email,
    bool Ativo,
    IReadOnlyList<string> Perfis);
