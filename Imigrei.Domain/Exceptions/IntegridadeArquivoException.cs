namespace Imigrei.Domain.Exceptions;

/// <summary>
/// O conteúdo lido do storage não bate com o hash registrado no upload.
/// Não é erro do cliente: é sinal de corrupção ou adulteração no servidor.
/// </summary>
public class IntegridadeArquivoException(Guid arquivoId)
    : Exception($"Falha de integridade no arquivo {arquivoId}: o conteúdo não corresponde ao hash registrado.")
{
    public Guid ArquivoId { get; } = arquivoId;
}
