using Imigrei.Domain.Enums;

namespace Imigrei.Application.Common.Relatorios;

/// <summary>Nomes legíveis dos enums, para quem lê o relatório impresso.</summary>
public static class RotulosRelatorio
{
    public static string De(StatusEmissaoCPFEnum status) => status switch
    {
        StatusEmissaoCPFEnum.NaoAplicavel => "Não aplicável",
        StatusEmissaoCPFEnum.Pendente => "Pendente",
        StatusEmissaoCPFEnum.FormularioPreenchido => "Formulário preenchido",
        StatusEmissaoCPFEnum.AguardandoDocumentacao => "Aguardando documentação",
        StatusEmissaoCPFEnum.Concluido => "Concluído",
        _ => status.ToString(),
    };

    public static string De(TipoAcaoImigranteEnum tipo) => tipo switch
    {
        TipoAcaoImigranteEnum.NovoPedido => "Novo pedido",
        TipoAcaoImigranteEnum.AtualizacaoDados => "Atualização de dados",
        TipoAcaoImigranteEnum.CancelamentoPedido => "Cancelamento de pedido",
        TipoAcaoImigranteEnum.FinalizacaoPedido => "Finalização de pedido",
        TipoAcaoImigranteEnum.Outros => "Outros",
        _ => tipo.ToString(),
    };

    public static string De(TipoArquivoImigranteEnum tipo) => tipo switch
    {
        TipoArquivoImigranteEnum.Passaporte => "Passaporte",
        TipoArquivoImigranteEnum.Rnm => "RNM",
        TipoArquivoImigranteEnum.ComprovanteResidencia => "Comprovante de residência",
        TipoArquivoImigranteEnum.CertidaoNascimento => "Certidão de nascimento",
        TipoArquivoImigranteEnum.CertidaoCasamento => "Certidão de casamento",
        TipoArquivoImigranteEnum.ComprovanteRenda => "Comprovante de renda",
        TipoArquivoImigranteEnum.FotoDocumento => "Foto para documento",
        TipoArquivoImigranteEnum.Outro => "Outro",
        _ => tipo.ToString(),
    };
}
