using Imigrei.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Imigrei.Domain.Entities.Identidade;

/// <summary>
/// Token de renovação de sessão. Agregado próprio: concentra as regras de
/// validade, revogação e rotação, sem depender de nenhum detalhe de persistência
/// ou do provedor de identidade.
/// </summary>
/// <remarks>
/// O valor bruto do token nunca é persistido — guarda-se apenas o hash SHA-256,
/// de modo que um vazamento do banco não permita renovar sessões.
/// </remarks>
public class RefreshToken
{
    public Guid Id { get; private set; }

    /// <summary>Identificador do usuário dono da sessão (o mesmo do provedor de identidade).</summary>
    public Guid UsuarioId { get; private set; }

    /// <summary>Hash SHA-256 (hexadecimal) do valor bruto do token.</summary>
    public string TokenHash { get; private set; } = string.Empty;

    public DateTime CriadoEm { get; private set; }

    public DateTime ExpiraEm { get; private set; }

    public DateTime? RevogadoEm { get; private set; }

    /// <summary>Hash do token que substituiu este, quando houve rotação.</summary>
    public string? SubstituidoPorTokenHash { get; private set; }

    public string? CriadoPorIp { get; private set; }

    public bool Expirado => DateTime.UtcNow >= ExpiraEm;

    public bool Revogado => RevogadoEm is not null;

    public bool Ativo => !Revogado && !Expirado;

    protected RefreshToken() { }

    private RefreshToken(Guid usuarioId, string tokenHash, DateTime expiraEm, string? criadoPorIp)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        TokenHash = tokenHash;
        CriadoEm = DateTime.UtcNow;
        ExpiraEm = expiraEm;
        CriadoPorIp = criadoPorIp;
    }

    /// <summary>
    /// Cria o token a partir do seu valor bruto, que é descartado após o hash.
    /// </summary>
    public static RefreshToken Criar(Guid usuarioId, string tokenBruto, TimeSpan validade, string? criadoPorIp = null)
    {
        if (usuarioId == Guid.Empty)
            throw new DomainException("Refresh token precisa estar vinculado a um usuário.");

        if (string.IsNullOrWhiteSpace(tokenBruto))
            throw new DomainException("Valor do refresh token não pode ser vazio.");

        if (validade <= TimeSpan.Zero)
            throw new DomainException("A validade do refresh token deve ser positiva.");

        return new RefreshToken(usuarioId, GerarHash(tokenBruto), DateTime.UtcNow.Add(validade), criadoPorIp);
    }

    /// <summary>
    /// Revoga a sessão. Idempotente: revogar duas vezes não é um erro de domínio.
    /// </summary>
    public void Revogar()
    {
        if (Revogado) return;
        RevogadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Rotaciona o token: revoga este e aponta para o sucessor. Só um token ativo
    /// pode ser rotacionado — reaproveitar um token já usado é tentativa de replay.
    /// </summary>
    public void Rotacionar(RefreshToken substituto)
    {
        ArgumentNullException.ThrowIfNull(substituto);

        if (!Ativo)
            throw new CredenciaisInvalidasException("Refresh token inválido ou expirado.");

        if (substituto.UsuarioId != UsuarioId)
            throw new DomainException("O token substituto pertence a outro usuário.");

        Revogar();
        SubstituidoPorTokenHash = substituto.TokenHash;
    }

    /// <summary>Hash SHA-256 em hexadecimal — a forma sob a qual o token é buscado e guardado.</summary>
    public static string GerarHash(string tokenBruto) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenBruto)));
}
