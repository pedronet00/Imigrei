
using Imigrei.Domain.Entities.Imigrantes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imigrei.Infra.Data.Configurations;

public class ImigranteConfiguration : IEntityTypeConfiguration<Imigrante>
{
    public void Configure(EntityTypeBuilder<Imigrante> builder)
    {
        builder.ToTable("imigrantes");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Nome).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Email).HasMaxLength(150).IsRequired();
        builder.Property(i => i.CidadeOrigem).HasMaxLength(100).IsRequired();
        builder.Property(i => i.PaisOrigem).HasMaxLength(100).IsRequired();
        builder.Property(i => i.IdiomaPrincipal).HasMaxLength(50).IsRequired();
        builder.Property(i => i.OcupacaoPrincipal).HasMaxLength(150).IsRequired();
        builder.Property(i => i.NomeMae).HasMaxLength(200).IsRequired();
        builder.Property(i => i.NomePai).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Telefone).HasMaxLength(20).IsRequired();
        builder.Property(i => i.Cep).HasMaxLength(9).IsRequired();
        builder.Property(i => i.CepTrabalho).HasMaxLength(9).IsRequired();
        builder.Property(i => i.Uf).HasMaxLength(2).IsRequired();
        builder.Property(i => i.UfTrabalho).HasMaxLength(2).IsRequired();
        builder.Property(i => i.UfEntradaBrasil).HasMaxLength(2).IsRequired();
        builder.Property(i => i.Cpf).HasMaxLength(11);

        builder.Property(i => i.EstadoCivil).HasConversion<string>().HasMaxLength(30);
        builder.Property(i => i.StatusEmissaoCPF).HasConversion<string>().HasMaxLength(30);
        builder.Property(i => i.DocumentoViagem).HasConversion<string>().HasMaxLength(30);
        builder.Property(i => i.MeioEntradaBrasil).HasConversion<string>().HasMaxLength(30);

        // Cpf é um Value Object — mapeado como Owned Type
        //builder.OwnsOne(i => i.Cpf, cpf =>
        //{
        //    cpf.Property(c => c.Valor)
        //       .HasColumnName("cpf")
        //       .HasMaxLength(11);
        //});

        builder.Property(i => i.DataCadastro).IsRequired();

        builder.HasIndex(i => i.Email).IsUnique();

        // "Novos este mês" do dashboard filtra por esta coluna.
        builder.HasIndex(i => i.DataCadastro);
    }
}
