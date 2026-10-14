using Imigrei.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Imigrei.Application.DTOs;

public record ImigranteAcaoRequestDto(
    [Required, StringLength(500, MinimumLength = 1)] string Acao,
    [Required] TipoAcaoImigranteEnum TipoAcao);
