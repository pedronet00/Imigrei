using System.ComponentModel.DataAnnotations;

namespace Imigrei.Application.DTOs.Auth;

/// <param name="Perfil">Opcional. Quando omitido, o usuário é criado como "Imigrante".</param>
public record RegistrarRequestDto(
    [Required, StringLength(200, MinimumLength = 3)] string Nome,
    [Required, EmailAddress, StringLength(150)] string Email,
    [Required, StringLength(100, MinimumLength = 8)] string Senha,
    [Required] string ConfirmacaoSenha,
    string? Perfil = null);
