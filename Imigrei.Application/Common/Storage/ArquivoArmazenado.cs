namespace Imigrei.Application.Common.Storage;

/// <summary>
/// Resultado da gravação. Tamanho e hash vêm do que foi realmente escrito —
/// não do que o cliente declarou no cabeçalho da requisição.
/// </summary>
public sealed record ArquivoArmazenado(
    string NomeArmazenado,
    string CaminhoRelativo,
    long TamanhoBytes,
    string HashSha256);
