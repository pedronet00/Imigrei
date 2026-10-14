using Imigrei.Application.DTOs.Auth;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Constants;
using Imigrei.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imigrei.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>Cria uma conta e já devolve a sessão autenticada.</summary>
    /// <remarks>Aberto ao público, mas só um administrador pode escolher o perfil.</remarks>
    [HttpPost("registrar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Registrar(
        [FromBody] RegistrarRequestDto dto, CancellationToken cancellationToken)
    {
        if (dto.Perfil is not null && !User.IsInRole(PerfisUsuario.Administrador))
            return Forbid();

        try
        {
            var resposta = await _service.RegistrarAsync(dto, cancellationToken);
            return Created(string.Empty, resposta);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        => Ok(await _service.LoginAsync(dto, cancellationToken));

    /// <summary>Troca um refresh token válido por um novo par de tokens (rotação).</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Refresh(
        [FromBody] RefreshTokenRequestDto dto, CancellationToken cancellationToken)
        => Ok(await _service.RenovarAsync(dto, cancellationToken));

    /// <summary>Revoga o refresh token informado. Idempotente.</summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequestDto dto, CancellationToken cancellationToken)
    {
        await _service.LogoutAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>Altera a senha do usuário autenticado e encerra as demais sessões.</summary>
    [HttpPost("alterar-senha")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarSenha(
        [FromBody] AlterarSenhaRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _service.AlterarSenhaAsync(dto, cancellationToken);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("eu")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioResponseDto>> Eu(CancellationToken cancellationToken)
        => Ok(await _service.ObterUsuarioAtualAsync(cancellationToken));
}
