using System.ComponentModel.DataAnnotations;

namespace Imigrei.Application.DTOs.Auth;

public record RefreshTokenRequestDto([Required] string RefreshToken);
