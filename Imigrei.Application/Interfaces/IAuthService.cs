using Imigrei.Application.DTOs.Auth;

namespace Imigrei.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegistrarRequestDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RenovarAsync(RefreshTokenRequestDto dto, CancellationToken cancellationToken = default);
    Task LogoutAsync(RefreshTokenRequestDto dto, CancellationToken cancellationToken = default);
    Task AlterarSenhaAsync(AlterarSenhaRequestDto dto, CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto> ObterUsuarioAtualAsync(CancellationToken cancellationToken = default);
}
