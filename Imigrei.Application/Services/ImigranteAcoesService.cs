using Imigrei.Application.DTOs;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Exceptions;
using Imigrei.Domain.Interfaces;

namespace Imigrei.Application.Services;

public class ImigranteAcoesService : IImigranteAcoesService
{
    private readonly IImigranteAcoesRepository _repository;
    private readonly IImigranteRepository _imigranteRepository;

    public ImigranteAcoesService(
        IImigranteAcoesRepository repository,
        IImigranteRepository imigranteRepository)
    {
        _repository = repository;
        _imigranteRepository = imigranteRepository;
    }

    public async Task<IEnumerable<ImigranteAcaoResponseDto>> GetByImigranteIdAsync(
        Guid imigranteId, CancellationToken cancellationToken = default)
    {
        var acoes = await _repository.GetByImigranteIdAsync(imigranteId, cancellationToken);
        return acoes.Select(ImigranteAcaoResponseDto.FromEntity);
    }

    public async Task<ImigranteAcaoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var acao = await _repository.GetByIdAsync(id, cancellationToken);
        return acao is null ? null : ImigranteAcaoResponseDto.FromEntity(acao);
    }

    public async Task<ImigranteAcaoResponseDto> CreateAsync(
        Guid imigranteId, ImigranteAcaoRequestDto dto, CancellationToken cancellationToken = default)
    {
        // Sem o imigrante a ação não tem a quem pertencer; o erro fica claro
        // aqui em vez de virar violação de chave estrangeira no banco.
        _ = await _imigranteRepository.GetByIdAsync(imigranteId, cancellationToken)
            ?? throw new DomainException("Imigrante não encontrado.");

        var acao = new ImigranteAcoes(imigranteId, dto.Acao, dto.TipoAcao);

        await _repository.AddAsync(acao, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ImigranteAcaoResponseDto.FromEntity(acao);
    }

    public async Task UpdateAsync(Guid id, ImigranteAcaoRequestDto dto, CancellationToken cancellationToken = default)
    {
        var acao = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Ação não encontrada.");

        // Muta o agregado já rastreado — nunca anexar uma segunda instância
        // com a mesma chave.
        acao.Atualizar(dto.Acao, dto.TipoAcao);

        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var acao = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Ação não encontrada.");

        _repository.Delete(acao);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
