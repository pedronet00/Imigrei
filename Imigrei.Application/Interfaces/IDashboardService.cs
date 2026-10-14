using Imigrei.Application.DTOs.Dashboard;

namespace Imigrei.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponseDto> ObterAsync(CancellationToken cancellationToken = default);
}
