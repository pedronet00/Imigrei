namespace Imigrei.Infrastructure.Authentication;

/// <summary>Configuração do emissor de tokens, lida da seção "Jwt".</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>Chave simétrica de assinatura. Mínimo de 32 caracteres (HMAC-SHA256).</summary>
    public string SecretKey { get; set; } = string.Empty;

    public int AccessTokenMinutos { get; set; } = 15;

    public int RefreshTokenDias { get; set; } = 7;
}
