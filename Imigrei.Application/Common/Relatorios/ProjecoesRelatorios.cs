using Imigrei.Domain.Enums;

namespace Imigrei.Application.Common.Relatorios;

/// <summary>Linha do cadastro de imigrantes, só com o que os relatórios exibem.</summary>
public sealed record ImigranteRelatorio(
    Guid Id,
    string Nome,
    string PaisOrigem,
    string Email,
    string Telefone,
    string Cidade,
    string Uf,
    StatusEmissaoCPFEnum StatusEmissaoCPF,
    DateTime DataCadastro);

public sealed record NacionalidadeRelatorio(string PaisOrigem, int Total);

/// <summary>Ação do histórico já cruzada com o imigrante no banco.</summary>
public sealed record AcaoRelatorio(
    Guid AcaoId,
    Guid ImigranteId,
    string NomeImigrante,
    string PaisOrigem,
    DateTime DataRegistro,
    string Acao,
    TipoAcaoImigranteEnum TipoAcao);

/// <summary>Metadados de um documento enviado — nunca o conteúdo.</summary>
public sealed record DocumentoRelatorio(
    string NomeImigrante,
    TipoArquivoImigranteEnum TipoArquivo,
    string NomeOriginal,
    long TamanhoBytes,
    DateTime DataUpload);
