using Imigrei.Application.Common.Relatorios;
using Imigrei.Application.DTOs.Relatorios;
using Imigrei.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imigrei.API.Controllers;

/// <summary>
/// Relatórios gerenciais para download. Todos aceitam
/// <c>?formato=pdf|excel&amp;dataInicio=aaaa-mm-dd&amp;dataFim=aaaa-mm-dd</c>;
/// as datas são opcionais e inclusivas, no horário de Brasília.
/// Restrito a quem opera o sistema, como o dashboard.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operacao")]
[Produces("application/pdf", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatoriosService _service;

    public RelatoriosController(IRelatoriosService service)
    {
        _service = service;
    }

    [HttpGet("imigrantes-cadastrados")]
    public async Task<IActionResult> ImigrantesCadastrados(
        [FromQuery] RelatorioRequestDto filtro, CancellationToken cancellationToken)
        => Arquivo(await _service.ImigrantesCadastradosAsync(filtro, cancellationToken));

    [HttpGet("imigrantes-por-nacionalidade")]
    public async Task<IActionResult> ImigrantesPorNacionalidade(
        [FromQuery] RelatorioRequestDto filtro, CancellationToken cancellationToken)
        => Arquivo(await _service.ImigrantesPorNacionalidadeAsync(filtro, cancellationToken));

    [HttpGet("atendimentos")]
    public async Task<IActionResult> Atendimentos(
        [FromQuery] RelatorioRequestDto filtro, CancellationToken cancellationToken)
        => Arquivo(await _service.AtendimentosAsync(filtro, cancellationToken));

    [HttpGet("pedidos-em-aberto")]
    public async Task<IActionResult> PedidosEmAberto(
        [FromQuery] RelatorioRequestDto filtro, CancellationToken cancellationToken)
        => Arquivo(await _service.PedidosEmAbertoAsync(filtro, cancellationToken));

    [HttpGet("documentos-enviados")]
    public async Task<IActionResult> DocumentosEnviados(
        [FromQuery] RelatorioRequestDto filtro, CancellationToken cancellationToken)
        => Arquivo(await _service.DocumentosEnviadosAsync(filtro, cancellationToken));

    [HttpGet("pendencias-cpf")]
    public async Task<IActionResult> PendenciasCpf(
        [FromQuery] RelatorioRequestDto filtro, CancellationToken cancellationToken)
        => Arquivo(await _service.PendenciasCpfAsync(filtro, cancellationToken));

    private FileContentResult Arquivo(ArquivoRelatorio arquivo)
        => File(arquivo.Conteudo, arquivo.ContentType, arquivo.NomeArquivo);
}
