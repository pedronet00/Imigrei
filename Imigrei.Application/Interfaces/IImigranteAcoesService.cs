using Imigrei.Application.DTOs;

namespace Imigrei.Application.Interfaces;

public interface IImigranteAcoesService
{
    /// <summary>Histórico do imigrante, do lançamento mais recente para o mais antigo.</summary>
    Task<IEnumerable<ImigranteAcaoResponseDto>> GetByImigranteIdAsync(
        Guid imigranteId, CancellationToken cancellationToken = default);

    Task<ImigranteAcaoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ImigranteAcaoResponseDto> CreateAsync(
        Guid imigranteId, ImigranteAcaoRequestDto dto, CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid id, ImigranteAcaoRequestDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
