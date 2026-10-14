using Imigrei.Application.DTOs;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imigrei.API.Controllers;

/// <summary>
/// Histórico de ações do imigrante. Leitura para qualquer usuário autenticado;
/// o lançamento e a manutenção dos registros são do Administrador.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImigranteAcoesController : ControllerBase
{
    private readonly IImigranteAcoesService _service;

    public ImigranteAcoesController(IImigranteAcoesService service)
    {
        _service = service;
    }

    /// <summary>Histórico do imigrante, do lançamento mais recente para o mais antigo.</summary>
    [HttpGet("imigrante/{imigranteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ImigranteAcaoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ImigranteAcaoResponseDto>>> GetByImigranteId(
        Guid imigranteId, CancellationToken cancellationToken)
        => Ok(await _service.GetByImigranteIdAsync(imigranteId, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ImigranteAcaoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImigranteAcaoResponseDto>> GetById(
        Guid id, CancellationToken cancellationToken)
    {
        var acao = await _service.GetByIdAsync(id, cancellationToken);
        return acao is null ? NotFound() : Ok(acao);
    }

    [HttpPost("imigrante/{imigranteId:guid}")]
    [Authorize(Policy = PerfisUsuario.Administrador)]
    [ProducesResponseType(typeof(ImigranteAcaoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImigranteAcaoResponseDto>> Create(
        Guid imigranteId, [FromBody] ImigranteAcaoRequestDto dto, CancellationToken cancellationToken)
    {
        var criada = await _service.CreateAsync(imigranteId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PerfisUsuario.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] ImigranteAcaoRequestDto dto, CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PerfisUsuario.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
