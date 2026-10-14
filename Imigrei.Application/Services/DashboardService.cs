using Imigrei.Application.DTOs.Dashboard;
using Imigrei.Application.Interfaces;

namespace Imigrei.Application.Services;

/// <summary>
/// Compõe o painel a partir das agregações do modelo de leitura.
/// </summary>
public class DashboardService : IDashboardService
{
    /// <summary>Quantos lançamentos do histórico aparecem em "atividade recente".</summary>
    private const int TamanhoAtividadeRecente = 5;

    private readonly IDashboardQuery _query;

    public DashboardService(IDashboardQuery query)
    {
        _query = query;
    }

    public async Task<DashboardResponseDto> ObterAsync(CancellationToken cancellationToken = default)
    {
        // Primeiro instante do mês corrente em UTC — mesma referência usada ao
        // gravar DataCadastro, então a contagem não escorrega por fuso.
        var agora = DateTime.UtcNow;
        var inicioDoMes = new DateTime(agora.Year, agora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // Sequencial de propósito: as consultas compartilham o mesmo contexto
        // do ORM, que não é seguro para uso concorrente.
        var totalImigrantes = await _query.ContarImigrantesAsync(cancellationToken);
        var novosEsteMes = await _query.ContarImigrantesCadastradosDesdeAsync(inicioDoMes, cancellationToken);
        var totalNacionalidades = await _query.ContarNacionalidadesAsync(cancellationToken);
        var totalDocumentos = await _query.ContarDocumentosAsync(cancellationToken);
        var porNacionalidade = await _query.ObterImigrantesPorNacionalidadeAsync(cancellationToken);
        var atividades = await _query.ObterAtividadesRecentesAsync(TamanhoAtividadeRecente, cancellationToken);

        return new DashboardResponseDto(
            totalImigrantes,
            novosEsteMes,
            totalNacionalidades,
            totalDocumentos,
            porNacionalidade.Select(ImigrantesPorNacionalidadeDto.FromProjecao).ToList(),
            atividades.Select(AtividadeRecenteDto.FromProjecao).ToList());
    }
}
