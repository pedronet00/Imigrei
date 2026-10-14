using Imigrei.Application.DTOs;

namespace Imigrei.Application.Interfaces;

public interface IImigranteService
{
    /// <summary>Detalhes completos do imigrante.</summary>
    Task<ImigranteDetalheResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Listagem resumida.</summary>
    Task<IEnumerable<ImigranteResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ImigranteResponseDto> CreateAsync(ImigranteRequestDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, ImigranteRequestDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}