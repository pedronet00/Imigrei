using Imigrei.Application.Common.Identidade;

namespace Imigrei.Application.Interfaces;

/// <summary>Porta para a emissão de tokens. O formato (JWT) é detalhe da infraestrutura.</summary>
public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiraEm) GerarAccessToken(UsuarioIdentidade usuario);

    /// <summary>Gera o valor bruto e aleatório de um refresh token.</summary>
    string GerarRefreshToken();

    /// <summary>Janela de validade configurada para os refresh tokens.</summary>
    TimeSpan ValidadeRefreshToken { get; }
}
