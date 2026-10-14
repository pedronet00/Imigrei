using Imigrei.Domain.Entities.Imigrantes;
using Imigrei.Domain.Enums;

namespace Imigrei.Application.DTOs;

/// <summary>
/// Resumo do imigrante, usado na listagem. Os detalhes completos estão em
/// <see cref="ImigranteDetalheResponseDto"/>.
/// </summary>
public record ImigranteResponseDto(
    Guid Id,
    string Nome,
    string? Cpf,
    DateTime DataNascimento,
    EstadoCivilEnum EstadoCivil,
    string Email,
    string PaisOrigem,
    DateTime DataEntradaBrasil,
    string Cidade,
    string Uf
)
{
    public static ImigranteResponseDto FromEntity(Imigrante i) =>
        new(i.Id, i.Nome, i.Cpf, i.DataNascimento, i.EstadoCivil, i.Email,
            i.PaisOrigem, i.DataEntradaBrasil, i.Cidade, i.Uf);
}
