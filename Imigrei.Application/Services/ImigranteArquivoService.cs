// Imigrei.Application/Services/ImigranteArquivoService.cs
using Imigrei.Application.DTOs;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Enums;
using Imigrei.Domain.Exceptions;
using Imigrei.Domain.Interfaces;
using System.Security.Cryptography;

namespace Imigrei.Application.Services;

public class ImigranteArquivoService : IImigranteArquivoService
{
    private readonly IImigranteArquivoRepository _repository;
    private readonly IImigranteRepository _imigranteRepository;
    private readonly IArquivoStorage _storage;

    public ImigranteArquivoService(
        IImigranteArquivoRepository repository,
        IImigranteRepository imigranteRepository,
        IArquivoStorage storage)
    {
        _repository = repository;
        _imigranteRepository = imigranteRepository;
        _storage = storage;
    }

    public async Task<IEnumerable<ImigranteArquivoResponseDto>> GetByImigranteIdAsync(
        Guid imigranteId, CancellationToken cancellationToken = default)
    {
        var arquivos = await _repository.GetByImigranteIdAsync(imigranteId, cancellationToken);
        return arquivos.Select(ImigranteArquivoResponseDto.FromEntity);
    }

    public async Task<ImigranteArquivoResponseDto> UploadAsync(
        Guid imigranteId, Stream conteudo, string nomeOriginal, string contentType,
        long tamanhoBytes, TipoArquivoImigranteEnum tipoArquivo = TipoArquivoImigranteEnum.Outro,
        CancellationToken cancellationToken = default)
    {
        _ = await _imigranteRepository.GetByIdAsync(imigranteId, cancellationToken)
            ?? throw new DomainException("Imigrante não encontrado.");

        // Rejeita extensão e tamanho antes de gravar qualquer byte, usando as
        // regras do próprio domínio.
        ImigranteArquivo.ValidarAntesDeArmazenar(nomeOriginal, tamanhoBytes);

        var armazenado = await _storage.SalvarAsync(
            conteudo, nomeOriginal, imigranteId, ImigranteArquivo.TamanhoMaximoBytes, cancellationToken);

        try
        {
            // Tamanho e hash vêm do que foi escrito, não do que o cliente declarou.
            var arquivo = new ImigranteArquivo(
                imigranteId, nomeOriginal, armazenado.NomeArmazenado, armazenado.CaminhoRelativo,
                contentType, armazenado.TamanhoBytes, armazenado.HashSha256, tipoArquivo);

            await _repository.AddAsync(arquivo, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ImigranteArquivoResponseDto.FromEntity(arquivo);
        }
        catch
        {
            // Sem registro no banco, o arquivo em disco seria órfão.
            _storage.Excluir(armazenado.CaminhoRelativo);
            throw;
        }
    }

    /// <summary>
    /// Lê o conteúdo e só o entrega se o SHA-256 bater com o registrado no upload.
    /// </summary>
    public async Task<(Stream Conteudo, string ContentType, string NomeOriginal)> DownloadAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var arquivo = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Arquivo não encontrado.");

        // Carrega na memória para conferir o hash e ainda assim devolver um
        // stream legível de uma só leitura do disco. Cabe: o teto é 10 MB.
        var buffer = new MemoryStream();
        await using (var origem = await _storage.AbrirLeituraAsync(arquivo.CaminhoRelativo, cancellationToken))
        {
            await origem.CopyToAsync(buffer, cancellationToken);
        }

        buffer.Position = 0;
        var hashRecalculado = Convert.ToHexString(await SHA256.HashDataAsync(buffer, cancellationToken));

        // Lança IntegridadeArquivoException se o conteúdo em disco não for o mesmo.
        arquivo.GarantirIntegridade(hashRecalculado);

        buffer.Position = 0;
        return (buffer, arquivo.ContentType, arquivo.NomeOriginal);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var arquivo = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Arquivo não encontrado.");

        _repository.Delete(arquivo);
        await _repository.SaveChangesAsync(cancellationToken);

        _storage.Excluir(arquivo.CaminhoRelativo);
    }
}
