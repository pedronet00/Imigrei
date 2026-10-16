using System.Globalization;
using Imigrei.Application.Common.Relatorios;
using Imigrei.Application.DTOs.Relatorios;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Enums;
using Imigrei.Domain.Exceptions;

namespace Imigrei.Application.Services;

/// <summary>
/// Monta cada relatório (colunas, linhas, resumo) a partir do modelo de
/// leitura e entrega ao exportador do formato pedido.
/// </summary>
public class RelatoriosService : IRelatoriosService
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Status em que o CPF ainda depende de alguma providência.</summary>
    private static readonly StatusEmissaoCPFEnum[] StatusCpfPendentes =
    [
        StatusEmissaoCPFEnum.Pendente,
        StatusEmissaoCPFEnum.FormularioPreenchido,
        StatusEmissaoCPFEnum.AguardandoDocumentacao,
    ];

    private static readonly TipoAcaoImigranteEnum[] TiposEventoPedido =
    [
        TipoAcaoImigranteEnum.NovoPedido,
        TipoAcaoImigranteEnum.CancelamentoPedido,
        TipoAcaoImigranteEnum.FinalizacaoPedido,
    ];

    private readonly IRelatoriosQuery _query;
    private readonly IReadOnlyDictionary<FormatoRelatorio, IRelatorioExportador> _exportadores;

    public RelatoriosService(IRelatoriosQuery query, IEnumerable<IRelatorioExportador> exportadores)
    {
        _query = query;
        _exportadores = exportadores.ToDictionary(e => e.Formato);
    }

    public async Task<ArquivoRelatorio> ImigrantesCadastradosAsync(
        RelatorioRequestDto filtro, CancellationToken cancellationToken = default)
    {
        var periodo = PeriodoRelatorio.Criar(filtro.DataInicio, filtro.DataFim);

        var imigrantes = await _query.ObterImigrantesCadastradosAsync(
            periodo.InicioUtc, periodo.FimExclusivoUtc, cancellationToken);

        var relatorio = new Relatorio(
            "Imigrantes cadastrados",
            "imigrantes-cadastrados",
            periodo,
            [
                new("Data de cadastro", TipoColunaRelatorio.DataHora, 1.3f),
                new("Nome", Peso: 2.2f),
                new("País de origem", Peso: 1.3f),
                new("E-mail", Peso: 2f),
                new("Telefone", Peso: 1.2f),
                new("Cidade/UF", Peso: 1.4f),
                new("Status do CPF", Peso: 1.4f),
            ],
            imigrantes.Select(i => new object?[]
            {
                HorarioBrasilia.DeUtc(i.DataCadastro),
                i.Nome,
                i.PaisOrigem,
                i.Email,
                i.Telefone,
                $"{i.Cidade}/{i.Uf}",
                RotulosRelatorio.De(i.StatusEmissaoCPF),
            }).ToList(),
            [
                new("Total de imigrantes cadastrados", Numero(imigrantes.Count)),
                new("Nacionalidades distintas", Numero(imigrantes.Select(i => i.PaisOrigem).Distinct().Count())),
            ]);

        return Exportar(relatorio, filtro.Formato);
    }

    public async Task<ArquivoRelatorio> ImigrantesPorNacionalidadeAsync(
        RelatorioRequestDto filtro, CancellationToken cancellationToken = default)
    {
        var periodo = PeriodoRelatorio.Criar(filtro.DataInicio, filtro.DataFim);

        var nacionalidades = await _query.ObterImigrantesPorNacionalidadeAsync(
            periodo.InicioUtc, periodo.FimExclusivoUtc, cancellationToken);

        var total = nacionalidades.Sum(n => n.Total);

        var relatorio = new Relatorio(
            "Imigrantes por nacionalidade",
            "imigrantes-por-nacionalidade",
            periodo,
            [
                new("País de origem", Peso: 3f),
                new("Imigrantes", TipoColunaRelatorio.Inteiro),
                new("Participação", TipoColunaRelatorio.Percentual),
            ],
            nacionalidades.Select(n => new object?[]
            {
                n.PaisOrigem,
                n.Total,
                total == 0 ? 0m : (decimal)n.Total / total,
            }).ToList(),
            [
                new("Total de imigrantes", Numero(total)),
                new("Nacionalidades distintas", Numero(nacionalidades.Count)),
            ]);

        return Exportar(relatorio, filtro.Formato);
    }

    public async Task<ArquivoRelatorio> AtendimentosAsync(
        RelatorioRequestDto filtro, CancellationToken cancellationToken = default)
    {
        var periodo = PeriodoRelatorio.Criar(filtro.DataInicio, filtro.DataFim);

        var acoes = await _query.ObterAcoesAsync(
            periodo.InicioUtc, periodo.FimExclusivoUtc, [], cancellationToken);

        var resumo = new List<ItemResumoRelatorio>
        {
            new("Total de atendimentos", Numero(acoes.Count)),
            new("Imigrantes atendidos", Numero(acoes.Select(a => a.ImigranteId).Distinct().Count())),
        };
        resumo.AddRange(acoes
            .GroupBy(a => a.TipoAcao)
            .OrderBy(g => g.Key)
            .Select(g => new ItemResumoRelatorio(RotulosRelatorio.De(g.Key), Numero(g.Count()))));

        var relatorio = new Relatorio(
            "Atendimentos",
            "atendimentos",
            periodo,
            [
                new("Data", TipoColunaRelatorio.DataHora, 1.2f),
                new("Imigrante", Peso: 2f),
                new("País de origem", Peso: 1.2f),
                new("Tipo", Peso: 1.5f),
                new("Descrição", Peso: 3.5f),
            ],
            acoes.Select(a => new object?[]
            {
                HorarioBrasilia.DeUtc(a.DataRegistro),
                a.NomeImigrante,
                a.PaisOrigem,
                RotulosRelatorio.De(a.TipoAcao),
                a.Acao,
            }).ToList(),
            resumo);

        return Exportar(relatorio, filtro.Formato);
    }

    public async Task<ArquivoRelatorio> PedidosEmAbertoAsync(
        RelatorioRequestDto filtro, CancellationToken cancellationToken = default)
    {
        var periodo = PeriodoRelatorio.Criar(filtro.DataInicio, filtro.DataFim);

        // Sem filtro de data aqui: um pedido aberto no período pode ter sido
        // encerrado depois dele, e um encerramento só se casa com o pedido
        // certo olhando a sequência inteira do imigrante.
        var eventos = await _query.ObterAcoesAsync(null, null, TiposEventoPedido, cancellationToken);

        var hoje = HorarioBrasilia.Hoje();

        var abertos = PedidosNaoEncerrados(eventos)
            .Where(p => (periodo.InicioUtc is null || p.DataRegistro >= periodo.InicioUtc)
                     && (periodo.FimExclusivoUtc is null || p.DataRegistro < periodo.FimExclusivoUtc))
            .OrderBy(p => p.DataRegistro)
            .ToList();

        var linhas = abertos.Select(p =>
        {
            var abertura = HorarioBrasilia.DeUtc(p.DataRegistro);
            return new object?[]
            {
                abertura,
                p.NomeImigrante,
                p.PaisOrigem,
                p.Acao,
                hoje.DayNumber - DateOnly.FromDateTime(abertura).DayNumber,
            };
        }).ToList();

        var relatorio = new Relatorio(
            "Pedidos em aberto",
            "pedidos-em-aberto",
            periodo,
            [
                new("Aberto em", TipoColunaRelatorio.DataHora, 1.2f),
                new("Imigrante", Peso: 2f),
                new("País de origem", Peso: 1.2f),
                new("Descrição do pedido", Peso: 3.5f),
                new("Dias em aberto", TipoColunaRelatorio.Inteiro, 0.9f),
            ],
            linhas,
            [
                new("Pedidos em aberto", Numero(abertos.Count)),
                new("Imigrantes com pedido em aberto", Numero(abertos.Select(p => p.ImigranteId).Distinct().Count())),
                new("Maior tempo em aberto (dias)", linhas.Count == 0 ? "-" : Numero(linhas.Max(l => (int)l[4]!))),
            ]);

        return Exportar(relatorio, filtro.Formato);
    }

    public async Task<ArquivoRelatorio> DocumentosEnviadosAsync(
        RelatorioRequestDto filtro, CancellationToken cancellationToken = default)
    {
        var periodo = PeriodoRelatorio.Criar(filtro.DataInicio, filtro.DataFim);

        var documentos = await _query.ObterDocumentosEnviadosAsync(
            periodo.InicioUtc, periodo.FimExclusivoUtc, cancellationToken);

        var resumo = new List<ItemResumoRelatorio>
        {
            new("Total de documentos", Numero(documentos.Count)),
            new("Volume enviado", Tamanho(documentos.Sum(d => d.TamanhoBytes))),
        };
        resumo.AddRange(documentos
            .GroupBy(d => d.TipoArquivo)
            .OrderBy(g => g.Key)
            .Select(g => new ItemResumoRelatorio(RotulosRelatorio.De(g.Key), Numero(g.Count()))));

        var relatorio = new Relatorio(
            "Documentos enviados",
            "documentos-enviados",
            periodo,
            [
                new("Enviado em", TipoColunaRelatorio.DataHora, 1.2f),
                new("Imigrante", Peso: 2f),
                new("Tipo de documento", Peso: 1.6f),
                new("Arquivo", Peso: 2.6f),
                new("Tamanho", Peso: 0.9f),
            ],
            documentos.Select(d => new object?[]
            {
                HorarioBrasilia.DeUtc(d.DataUpload),
                d.NomeImigrante,
                RotulosRelatorio.De(d.TipoArquivo),
                d.NomeOriginal,
                Tamanho(d.TamanhoBytes),
            }).ToList(),
            resumo);

        return Exportar(relatorio, filtro.Formato);
    }

    public async Task<ArquivoRelatorio> PendenciasCpfAsync(
        RelatorioRequestDto filtro, CancellationToken cancellationToken = default)
    {
        var periodo = PeriodoRelatorio.Criar(filtro.DataInicio, filtro.DataFim);

        var imigrantes = await _query.ObterImigrantesComStatusCpfAsync(
            periodo.InicioUtc, periodo.FimExclusivoUtc, StatusCpfPendentes, cancellationToken);

        var hoje = HorarioBrasilia.Hoje();

        var resumo = new List<ItemResumoRelatorio>
        {
            new("Imigrantes com CPF pendente", Numero(imigrantes.Count)),
        };
        resumo.AddRange(StatusCpfPendentes.Select(s => new ItemResumoRelatorio(
            RotulosRelatorio.De(s), Numero(imigrantes.Count(i => i.StatusEmissaoCPF == s)))));

        var relatorio = new Relatorio(
            "Pendências de CPF",
            "pendencias-cpf",
            periodo,
            [
                new("Cadastrado em", TipoColunaRelatorio.Data, 1.1f),
                new("Nome", Peso: 2.2f),
                new("País de origem", Peso: 1.3f),
                new("Status do CPF", Peso: 1.6f),
                new("Telefone", Peso: 1.2f),
                new("E-mail", Peso: 2f),
                new("Dias desde o cadastro", TipoColunaRelatorio.Inteiro, 1f),
            ],
            imigrantes.Select(i =>
            {
                var cadastro = HorarioBrasilia.DeUtc(i.DataCadastro);
                return new object?[]
                {
                    cadastro,
                    i.Nome,
                    i.PaisOrigem,
                    RotulosRelatorio.De(i.StatusEmissaoCPF),
                    i.Telefone,
                    i.Email,
                    hoje.DayNumber - DateOnly.FromDateTime(cadastro).DayNumber,
                };
            }).ToList(),
            resumo);

        return Exportar(relatorio, filtro.Formato);
    }

    /// <summary>
    /// Pedidos que nenhum cancelamento/finalização encerrou. As ações não
    /// apontam para o pedido que encerram, então cada encerramento fecha o
    /// pedido mais antigo ainda aberto do mesmo imigrante (fila FIFO).
    /// Encerramento sem pedido aberto é ignorado.
    /// </summary>
    private static IEnumerable<AcaoRelatorio> PedidosNaoEncerrados(IEnumerable<AcaoRelatorio> eventos)
    {
        foreach (var doImigrante in eventos.GroupBy(e => e.ImigranteId))
        {
            var abertos = new Queue<AcaoRelatorio>();

            foreach (var evento in doImigrante.OrderBy(e => e.DataRegistro))
            {
                if (evento.TipoAcao == TipoAcaoImigranteEnum.NovoPedido)
                    abertos.Enqueue(evento);
                else
                    abertos.TryDequeue(out _);
            }

            foreach (var pedido in abertos)
                yield return pedido;
        }
    }

    private ArquivoRelatorio Exportar(Relatorio relatorio, FormatoRelatorio formato)
        => _exportadores.TryGetValue(formato, out var exportador)
            ? exportador.Exportar(relatorio)
            : throw new DomainException($"Formato de relatório não suportado: {formato}.");

    private static string Numero(int valor) => valor.ToString("N0", PtBr);

    private static string Tamanho(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => (bytes / 1024d).ToString("N1", PtBr) + " KB",
        < 1024L * 1024 * 1024 => (bytes / (1024d * 1024)).ToString("N1", PtBr) + " MB",
        _ => (bytes / (1024d * 1024 * 1024)).ToString("N2", PtBr) + " GB",
    };
}
