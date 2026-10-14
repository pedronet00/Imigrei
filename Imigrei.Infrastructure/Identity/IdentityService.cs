using Imigrei.Application.Common.Identidade;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Imigrei.Infrastructure.Identity;

/// <summary>
/// Adapta o ASP.NET Core Identity à porta <see cref="IIdentityService"/>,
/// traduzindo <see cref="IdentityResult"/> em exceções de domínio.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken = default)
        => await _userManager.FindByEmailAsync(email) is not null;

    public async Task<UsuarioIdentidade> CriarUsuarioAsync(
        string nome, string email, string senha, string perfil, CancellationToken cancellationToken = default)
    {
        var usuario = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Email = email,
            UserName = email,
            CriadoEm = DateTime.UtcNow,
            Ativo = true,
        };

        var resultado = await _userManager.CreateAsync(usuario, senha);
        GarantirSucesso(resultado, "Não foi possível criar o usuário.");

        var resultadoPerfil = await _userManager.AddToRoleAsync(usuario, perfil);
        if (!resultadoPerfil.Succeeded)
        {
            // Sem transação distribuída entre UserManager e o perfil: desfaz o usuário.
            await _userManager.DeleteAsync(usuario);
            GarantirSucesso(resultadoPerfil, "Não foi possível atribuir o perfil ao usuário.");
        }

        return await ProjetarAsync(usuario);
    }

    public async Task<UsuarioIdentidade?> ValidarCredenciaisAsync(
        string email, string senha, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        if (usuario is null)
            return null;

        if (await _userManager.IsLockedOutAsync(usuario))
            throw new CredenciaisInvalidasException("Conta temporariamente bloqueada por excesso de tentativas.");

        if (!await _userManager.CheckPasswordAsync(usuario, senha))
        {
            await _userManager.AccessFailedAsync(usuario);
            return null;
        }

        await _userManager.ResetAccessFailedCountAsync(usuario);

        return await ProjetarAsync(usuario);
    }

    public async Task<UsuarioIdentidade?> ObterPorIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId.ToString());
        return usuario is null ? null : await ProjetarAsync(usuario);
    }

    public async Task AlterarSenhaAsync(
        Guid usuarioId, string senhaAtual, string novaSenha, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new CredenciaisInvalidasException("Usuário não encontrado.");

        var resultado = await _userManager.ChangePasswordAsync(usuario, senhaAtual, novaSenha);
        GarantirSucesso(resultado, "Não foi possível alterar a senha.");
    }

    private async Task<UsuarioIdentidade> ProjetarAsync(ApplicationUser usuario)
    {
        var perfis = await _userManager.GetRolesAsync(usuario);

        return new UsuarioIdentidade(
            usuario.Id,
            usuario.Nome,
            usuario.Email ?? string.Empty,
            usuario.Ativo,
            perfis.ToList());
    }

    private static void GarantirSucesso(IdentityResult resultado, string mensagemPadrao)
    {
        if (resultado.Succeeded) return;

        var erros = string.Join(" ", resultado.Errors.Select(e => e.Description));
        throw new DomainException(string.IsNullOrWhiteSpace(erros) ? mensagemPadrao : erros);
    }
}
