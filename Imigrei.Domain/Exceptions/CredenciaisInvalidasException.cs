namespace Imigrei.Domain.Exceptions;

/// <summary>
/// Falha de autenticação: credenciais incorretas, sessão expirada ou usuário inativo.
/// Distinta de <see cref="DomainException"/> para que a borda traduza em 401 e não em 400.
/// </summary>
public class CredenciaisInvalidasException(string message = "Credenciais inválidas.")
    : Exception(message)
{
}
