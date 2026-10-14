using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Enums;

namespace Imigrei.Application.DTOs;

/// <summary>
/// Retrato completo do imigrante, usado no endpoint de detalhes.
/// A listagem continua com <see cref="ImigranteResponseDto"/>, que é o resumo.
/// </summary>
public record ImigranteDetalheResponseDto(
    Guid Id,

    // Identificação
    string Nome,
    DateTime DataNascimento,
    EstadoCivilEnum EstadoCivil,
    string CidadeOrigem,
    string PaisOrigem,
    string IdiomaPrincipal,
    string Email,
    string Telefone,
    string OcupacaoPrincipal,

    // Filiação e CPF
    string NomeMae,
    string NomePai,
    StatusEmissaoCPFEnum StatusEmissaoCPF,
    string? Cpf,
    bool TaxaRnm,

    // Documento de viagem e entrada no Brasil
    DocumentoViagemEnum DocumentoViagem,
    string PaisEmissorDocumentoViagem,
    string NumeroDocumentoViagem,
    DateTime DataEntradaBrasil,
    MeioEntradaBrasilEnum MeioEntradaBrasil,
    string CidadeEntradaBrasil,
    string UfEntradaBrasil,

    // Endereço residencial
    string Cep,
    string Logradouro,
    int Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Uf,

    // Endereço de trabalho
    string CepTrabalho,
    string LogradouroTrabalho,
    int NumeroTrabalho,
    string? ComplementoTrabalho,
    string BairroTrabalho,
    string CidadeTrabalho,
    string UfTrabalho,

    DateTime DataCadastro)
{
    public static ImigranteDetalheResponseDto FromEntity(Imigrante i) =>
        new(i.Id,
            i.Nome, i.DataNascimento, i.EstadoCivil, i.CidadeOrigem, i.PaisOrigem,
            i.IdiomaPrincipal, i.Email, i.Telefone, i.OcupacaoPrincipal,
            i.NomeMae, i.NomePai, i.StatusEmissaoCPF, i.Cpf, i.TaxaRnm,
            i.DocumentoViagem, i.PaisEmissorDocumentoViagem, i.NumeroDocumentoViagem,
            i.DataEntradaBrasil, i.MeioEntradaBrasil, i.CidadeEntradaBrasil, i.UfEntradaBrasil,
            i.Cep, i.Logradouro, i.Numero, i.Complemento, i.Bairro, i.Cidade, i.Uf,
            i.CepTrabalho, i.LogradouroTrabalho, i.NumeroTrabalho, i.ComplementoTrabalho,
            i.BairroTrabalho, i.CidadeTrabalho, i.UfTrabalho,
            i.DataCadastro);
}
