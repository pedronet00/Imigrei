namespace Imigrei.Application.Interfaces;

/// <summary>Usuário associado à requisição em curso. Implementado na borda (API).</summary>
public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    bool EstaAutenticado { get; }
    IReadOnlyList<string> Perfis { get; }
    string? EnderecoIp { get; }
}
