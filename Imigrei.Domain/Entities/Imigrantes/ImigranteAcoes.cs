// Imigrei.Domain/Entities/Imigrantes/ImigranteAcoes.cs
using Imigrei.Domain.Enums;
using Imigrei.Domain.Exceptions;

namespace Imigrei.Domain.Entities.Imigrantes;

public class ImigranteAcoes
{
    public Guid Id { get; private set; }

    public Guid ImigranteId { get; private set; }
    public Imigrante Imigrante { get; private set; } = null!;

    public string Acao { get; private set; } = string.Empty;

    public TipoAcaoImigranteEnum TipoAcao { get; private set; }

    public DateTime DataRegistro { get; private set; }

    protected ImigranteAcoes() { }

    public ImigranteAcoes(
        Guid imigranteId,
        string acao,
        TipoAcaoImigranteEnum tipoAcao)
    {
        ImigranteId = imigranteId;
        Acao = acao;
        TipoAcao = tipoAcao;
        DataRegistro = DateTime.UtcNow;

        Validate();

        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Reescreve a ação registrada, revalidando. Muta a própria instância: o
    /// registro preserva sua identidade e a data original do lançamento.
    /// </summary>
    public void Atualizar(string acao, TipoAcaoImigranteEnum tipoAcao)
    {
        Acao = acao;
        TipoAcao = tipoAcao;

        Validate();
    }

    /// <summary>
    /// Valida todo o estado da entidade. Lança DomainException na primeira
    /// regra violada, listando o campo correspondente.
    /// </summary>
    private void Validate()
    {
        if (ImigranteId == Guid.Empty)
            throw new DomainException("ImigranteId é obrigatório.");

        if (string.IsNullOrWhiteSpace(Acao))
            throw new DomainException("Ação não pode ser vazia.");

        if (Acao.Length > 500)
            throw new DomainException("Ação não pode ter mais que 500 caracteres.");

        if (!Enum.IsDefined(typeof(TipoAcaoImigranteEnum), TipoAcao))
            throw new DomainException("Tipo de ação inválido.");
    }
}