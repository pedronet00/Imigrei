using Imigrei.Application.Common.Identidade;
using Imigrei.Application.DTOs.Auth;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Constants;
using Imigrei.Domain.Entities.Identidade;
using Imigrei.Domain.Exceptions;
using Imigrei.Domain.Interfaces;

namespace Imigrei.Application.Services;

/// <summary>
/// Orquestra os casos de uso de autenticação. Depende apenas de portas: o
/// provedor de identidade, o emissor de tokens e o repositório de refresh tokens.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;

    public AuthService(
        IIdentityService identityService,
        IJwtTokenGenerator tokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUser currentUser)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistrarRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Senha != dto.ConfirmacaoSenha)
            throw new DomainException("A confirmação de senha não confere.");

        var perfil = dto.Perfil ?? PerfisUsuario.Imigrante;

        if (!PerfisUsuario.EhValido(perfil))
            throw new DomainException($"Perfil '{perfil}' não é um perfil válido.");

        if (await _identityService.EmailExisteAsync(dto.Email, cancellationToken))
            throw new DomainException("Já existe um usuário cadastrado com este e-mail.");

        var usuario = await _identityService.CriarUsuarioAsync(
            dto.Nome, dto.Email, dto.Senha, perfil, cancellationToken);

        return await EmitirSessaoAsync(usuario, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _identityService.ValidarCredenciaisAsync(dto.Email, dto.Senha, cancellationToken)
            ?? throw new CredenciaisInvalidasException("E-mail ou senha inválidos.");

        if (!usuario.Ativo)
            throw new CredenciaisInvalidasException("Usuário inativo. Procure o administrador.");

        return await EmitirSessaoAsync(usuario, cancellationToken);
    }

    public async Task<AuthResponseDto> RenovarAsync(RefreshTokenRequestDto dto, CancellationToken cancellationToken = default)
    {
        var tokenArmazenado = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, cancellationToken)
            ?? throw new CredenciaisInvalidasException("Refresh token inválido ou expirado.");

        if (!tokenArmazenado.Ativo)
        {
            // Reuso de token já revogado indica possível vazamento: encerra todas as sessões.
            await RevogarSessoesAsync(tokenArmazenado.UsuarioId, cancellationToken);
            throw new CredenciaisInvalidasException("Refresh token inválido ou expirado.");
        }

        var usuario = await _identityService.ObterPorIdAsync(tokenArmazenado.UsuarioId, cancellationToken)
            ?? throw new CredenciaisInvalidasException("Usuário da sessão não existe mais.");

        if (!usuario.Ativo)
            throw new CredenciaisInvalidasException("Usuário inativo. Procure o administrador.");

        var novoRefreshToken = _tokenGenerator.GerarRefreshToken();
        var substituto = RefreshToken.Criar(
            usuario.Id, novoRefreshToken, _tokenGenerator.ValidadeRefreshToken, _currentUser.EnderecoIp);

        tokenArmazenado.Rotacionar(substituto);

        _refreshTokenRepository.Update(tokenArmazenado);
        await _refreshTokenRepository.AddAsync(substituto, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        var (accessToken, expiraEm) = _tokenGenerator.GerarAccessToken(usuario);

        return new AuthResponseDto(accessToken, expiraEm, novoRefreshToken, UsuarioResponseDto.FromUsuario(usuario));
    }

    public async Task LogoutAsync(RefreshTokenRequestDto dto, CancellationToken cancellationToken = default)
    {
        var tokenArmazenado = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, cancellationToken);

        // Logout é idempotente: um token desconhecido ou já revogado não é erro.
        if (tokenArmazenado is null || !tokenArmazenado.Ativo)
            return;

        tokenArmazenado.Revogar();
        _refreshTokenRepository.Update(tokenArmazenado);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task AlterarSenhaAsync(AlterarSenhaRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.NovaSenha != dto.ConfirmacaoNovaSenha)
            throw new DomainException("A confirmação da nova senha não confere.");

        var usuarioId = _currentUser.Id
            ?? throw new CredenciaisInvalidasException("É necessário estar autenticado.");

        await _identityService.AlterarSenhaAsync(usuarioId, dto.SenhaAtual, dto.NovaSenha, cancellationToken);

        // Trocar a senha encerra as demais sessões abertas.
        await RevogarSessoesAsync(usuarioId, cancellationToken);
    }

    public async Task<UsuarioResponseDto> ObterUsuarioAtualAsync(CancellationToken cancellationToken = default)
    {
        var usuarioId = _currentUser.Id
            ?? throw new CredenciaisInvalidasException("É necessário estar autenticado.");

        var usuario = await _identityService.ObterPorIdAsync(usuarioId, cancellationToken)
            ?? throw new CredenciaisInvalidasException("Usuário não encontrado.");

        return UsuarioResponseDto.FromUsuario(usuario);
    }

    private async Task<AuthResponseDto> EmitirSessaoAsync(UsuarioIdentidade usuario, CancellationToken cancellationToken)
    {
        var (accessToken, expiraEm) = _tokenGenerator.GerarAccessToken(usuario);
        var refreshTokenBruto = _tokenGenerator.GerarRefreshToken();

        var refreshToken = RefreshToken.Criar(
            usuario.Id, refreshTokenBruto, _tokenGenerator.ValidadeRefreshToken, _currentUser.EnderecoIp);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(accessToken, expiraEm, refreshTokenBruto, UsuarioResponseDto.FromUsuario(usuario));
    }

    private async Task RevogarSessoesAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        var ativos = await _refreshTokenRepository.GetAtivosPorUsuarioAsync(usuarioId, cancellationToken);

        foreach (var token in ativos)
        {
            token.Revogar();
            _refreshTokenRepository.Update(token);
        }

        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }
}
