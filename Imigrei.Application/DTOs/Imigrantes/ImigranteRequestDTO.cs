// Imigrei.Application/DTOs/ImigranteRequestDto.cs
using Imigrei.Domain.Enums;

namespace Imigrei.Application.DTOs;

public record ImigranteRequestDto(
    string Nome,
    DateTime DataNascimento,
    EstadoCivilEnum EstadoCivil,
    string CidadeOrigem,
    string PaisOrigem,
    string IdiomaPrincipal,
    string Email,
    string OcupacaoPrincipal,
    StatusEmissaoCPFEnum StatusEmissaoCPF,
    string? Cpf,
    string NomeMae,
    string NomePai,
    bool TaxaRnm,
    DocumentoViagemEnum DocumentoViagem,
    string PaisEmissorDocumentoViagem,
    string NumeroDocumentoViagem,
    DateTime DataEntradaBrasil,
    MeioEntradaBrasilEnum MeioEntradaBrasil,
    string CidadeEntradaBrasil,
    string UfEntradaBrasil,
    string Cep,
    string Logradouro,
    int Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Uf,
    string CepTrabalho,
    string LogradouroTrabalho,
    int NumeroTrabalho,
    string? ComplementoTrabalho,
    string BairroTrabalho,
    string CidadeTrabalho,
    string UfTrabalho,
    string Telefone
);