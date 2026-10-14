using Imigrei.Application.DTOs.Dashboard;
using Imigrei.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imigrei.API.Controllers;

/// <summary>
/// Painel gerencial. Restrito a quem opera o sistema: agrega a base inteira,
/// então não faz sentido para o perfil Imigrante.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operacao")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponseDto>> Obter(CancellationToken cancellationToken)
        => Ok(await _service.ObterAsync(cancellationToken));
}
