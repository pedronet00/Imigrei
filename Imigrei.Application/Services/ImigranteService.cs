using Imigrei.Application.DTOs;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Entities;
using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Exceptions;
using Imigrei.Domain.Interfaces;
using Imigrei.Domain.ValueObjects;

namespace Imigrei.Application.Services;

public class ImigranteService : IImigranteService
{
    private readonly IImigranteRepository _repository;

    public ImigranteService(IImigranteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ImigranteDetalheResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var imigrante = await _repository.GetByIdAsync(id, cancellationToken);
        return imigrante is null ? null : ImigranteDetalheResponseDto.FromEntity(imigrante);
    }

    public async Task<IEnumerable<ImigranteResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var imigrantes = await _repository.GetAllAsync(cancellationToken);
        return imigrantes.Select(ImigranteResponseDto.FromEntity);
    }

    public async Task<ImigranteResponseDto> CreateAsync(ImigranteRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsByEmailAsync(dto.Email, cancellationToken))
            throw new DomainException("Já existe um imigrante cadastrado com este e-mail.");

        var imigrante = new Imigrante(
            dto.Nome, dto.DataNascimento, dto.EstadoCivil, dto.CidadeOrigem, dto.PaisOrigem,
            dto.IdiomaPrincipal, dto.Email, dto.OcupacaoPrincipal, dto.StatusEmissaoCPF,
            dto.Cpf is null ? null : dto.Cpf, dto.NomeMae, dto.NomePai, dto.TaxaRnm,
            dto.DocumentoViagem, dto.PaisEmissorDocumentoViagem, dto.NumeroDocumentoViagem,
            dto.DataEntradaBrasil, dto.MeioEntradaBrasil, dto.CidadeEntradaBrasil, dto.UfEntradaBrasil,
            dto.Cep, dto.Logradouro, dto.Numero, dto.Complemento, dto.Bairro, dto.Cidade, dto.Uf,
            dto.CepTrabalho, dto.LogradouroTrabalho, dto.NumeroTrabalho, dto.ComplementoTrabalho,
            dto.BairroTrabalho, dto.CidadeTrabalho, dto.UfTrabalho, dto.Telefone);

        await _repository.AddAsync(imigrante, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ImigranteResponseDto.FromEntity(imigrante);
    }

    public async Task UpdateAsync(Guid id, ImigranteRequestDto dto, CancellationToken cancellationToken = default)
    {
        var imigrante = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Imigrante não encontrado.");

        // Muta o próprio agregado, que já está rastreado pelo contexto. Criar uma
        // segunda instância com o mesmo Id faria o ORM recusar o attach.
        imigrante.Atualizar(
            dto.Nome, dto.DataNascimento, dto.EstadoCivil, dto.CidadeOrigem, dto.PaisOrigem,
            dto.IdiomaPrincipal, dto.Email, dto.OcupacaoPrincipal, dto.StatusEmissaoCPF,
            dto.Cpf, dto.NomeMae, dto.NomePai, dto.TaxaRnm,
            dto.DocumentoViagem, dto.PaisEmissorDocumentoViagem, dto.NumeroDocumentoViagem,
            dto.DataEntradaBrasil, dto.MeioEntradaBrasil, dto.CidadeEntradaBrasil, dto.UfEntradaBrasil,
            dto.Cep, dto.Logradouro, dto.Numero, dto.Complemento, dto.Bairro, dto.Cidade, dto.Uf,
            dto.CepTrabalho, dto.LogradouroTrabalho, dto.NumeroTrabalho, dto.ComplementoTrabalho,
            dto.BairroTrabalho, dto.CidadeTrabalho, dto.UfTrabalho, dto.Telefone);

        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var imigrante = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException("Imigrante não encontrado.");

        _repository.Delete(imigrante);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}