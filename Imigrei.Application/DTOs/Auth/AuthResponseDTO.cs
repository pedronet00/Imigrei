namespace Imigrei.Application.DTOs.Auth;

public record AuthResponseDto(
    string AccessToken,
    DateTime ExpiraEm,
    string RefreshToken,
    UsuarioResponseDto Usuario)
{
    public string TokenType => "Bearer";
}
