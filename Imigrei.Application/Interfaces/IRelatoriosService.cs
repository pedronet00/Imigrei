using Imigrei.Application.Common.Relatorios;
using Imigrei.Application.DTOs.Relatorios;

namespace Imigrei.Application.Interfaces;

public interface IRelatoriosService
{
    Task<ArquivoRelatorio> ImigrantesCadastradosAsync(RelatorioRequestDto filtro, CancellationToken cancellationToken = default);

    Task<ArquivoRelatorio> ImigrantesPorNacionalidadeAsync(RelatorioRequestDto filtro, CancellationToken cancellationToken = default);

    Task<ArquivoRelatorio> AtendimentosAsync(RelatorioRequestDto filtro, CancellationToken cancellationToken = default);

    Task<ArquivoRelatorio> PedidosEmAbertoAsync(RelatorioRequestDto filtro, CancellationToken cancellationToken = default);

    Task<ArquivoRelatorio> DocumentosEnviadosAsync(RelatorioRequestDto filtro, CancellationToken cancellationToken = default);

    Task<ArquivoRelatorio> PendenciasCpfAsync(RelatorioRequestDto filtro, CancellationToken cancellationToken = default);
}
