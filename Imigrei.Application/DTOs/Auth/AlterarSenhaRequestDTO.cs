using System.ComponentModel.DataAnnotations;

namespace Imigrei.Application.DTOs.Auth;

public record AlterarSenhaRequestDto(
    [Required] string SenhaAtual,
    [Required, StringLength(100, MinimumLength = 8)] string NovaSenha,
    [Required] string ConfirmacaoNovaSenha);
