using Imigrei.Application.Common.Identidade;

namespace Imigrei.Application.Interfaces;

/// <summary>
/// Porta para o provedor de identidade (implementada na infraestrutura sobre o
/// ASP.NET Core Identity). Lida com credenciais e perfis — nunca com tokens.
/// </summary>
public interface IIdentityService
{
    Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken = default);

    Task<UsuarioIdentidade> CriarUsuarioAsync(
        string nome, string email, string senha, string perfil, CancellationToken cancellationToken = default);

    /// <summary>Retorna o usuário quando a senha confere; caso contrário, <c>null</c>.</summary>
    Task<UsuarioIdentidade?> ValidarCredenciaisAsync(
        string email, string senha, CancellationToken cancellationToken = default);

    Task<UsuarioIdentidade?> ObterPorIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task AlterarSenhaAsync(
        Guid usuarioId, string senhaAtual, string novaSenha, CancellationToken cancellationToken = default);
}
