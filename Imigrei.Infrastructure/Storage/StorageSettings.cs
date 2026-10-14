namespace Imigrei.Infra.Storage;

/// <summary>Configuração do armazenamento de arquivos, lida da seção "Storage".</summary>
public class StorageSettings
{
    public const string SectionName = "Storage";

    /// <summary>
    /// Raiz onde os documentos dos imigrantes são gravados. Aceita caminho do
    /// Windows em desenvolvimento e do Linux na VPS; se vazio, cai numa pasta
    /// ao lado do binário.
    /// </summary>
    public string ImigrantesPath { get; set; } = string.Empty;
}
