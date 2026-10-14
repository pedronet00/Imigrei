using System.ComponentModel.DataAnnotations;

namespace Imigrei.Application.DTOs.Auth;

public record LoginRequestDto(
    [Required, EmailAddress] string Email,
    [Required] string Senha);
