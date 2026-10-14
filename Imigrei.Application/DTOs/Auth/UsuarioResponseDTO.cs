using Imigrei.Application.Common.Identidade;

namespace Imigrei.Application.DTOs.Auth;

public record UsuarioResponseDto(
    Guid Id,
    string Nome,
    string Email,
    IReadOnlyList<string> Perfis)
{
    public static UsuarioResponseDto FromUsuario(UsuarioIdentidade u) =>
        new(u.Id, u.Nome, u.Email, u.Perfis);
}
