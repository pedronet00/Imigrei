// Imigrei.API/Controllers/ImigranteArquivosController.cs
using Imigrei.Application.DTOs;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imigrei.API.Controllers;

/// <summary>
/// Documentos sigilosos dos imigrantes. Toda a controller — inclusive a leitura
/// e o download — é restrita ao perfil Administrador.
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Policy = PerfisUsuario.Administrador)]
public class ImigranteArquivosController : ControllerBase
{
    private readonly IImigranteArquivoService _service;

    public ImigranteArquivosController(IImigranteArquivoService service)
    {
        _service = service;
    }

    [HttpGet("imigrante/{imigranteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ImigranteArquivoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ImigranteArquivoResponseDto>>> GetByImigranteId(
        Guid imigranteId, CancellationToken cancellationToken)
        => Ok(await _service.GetByImigranteIdAsync(imigranteId, cancellationToken));

    /// <summary>
    /// Envia um documento. O corpo leva só o arquivo: nome, content-type e
    /// tamanho saem dele próprio, e o SHA-256 é calculado na gravação.
    /// </summary>
    [HttpPost("imigrante/{imigranteId:guid}")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    [ProducesResponseType(typeof(ImigranteArquivoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImigranteArquivoResponseDto>> Upload(
        Guid imigranteId,
        [FromForm] IFormFile arquivo,
        CancellationToken cancellationToken)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { erro = "Nenhum arquivo enviado." });

        await using var stream = arquivo.OpenReadStream();

        var criado = await _service.UploadAsync(
            imigranteId, stream, arquivo.FileName, arquivo.ContentType,
            arquivo.Length, cancellationToken: cancellationToken);

        return CreatedAtAction(nameof(GetByImigranteId), new { imigranteId }, criado);
    }

    /// <summary>
    /// Baixa o documento. O conteúdo só é entregue se o hash recalculado bater
    /// com o registrado no upload.
    /// </summary>
    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var (conteudo, contentType, nomeOriginal) = await _service.DownloadAsync(id, cancellationToken);
        return File(conteudo, contentType, nomeOriginal);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
