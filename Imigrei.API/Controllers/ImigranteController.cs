// Imigrei.API/Controllers/ImigrantesController.cs
using Imigrei.Application.DTOs;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imigrei.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class ImigrantesController : ControllerBase
{
    private readonly IImigranteService _service;

    public ImigrantesController(IImigranteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImigranteResponseDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _service.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ImigranteDetalheResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImigranteDetalheResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var imigrante = await _service.GetByIdAsync(id, cancellationToken);
        return imigrante is null ? NotFound() : Ok(imigrante);
    }

    [HttpPost]
    [Authorize(Policy = "Operacao")]
    public async Task<ActionResult<ImigranteResponseDto>> Create(
        [FromBody] ImigranteRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var criado = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Operacao")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] ImigranteRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _service.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Administrador")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}