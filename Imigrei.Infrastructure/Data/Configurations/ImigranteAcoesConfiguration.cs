using Imigrei.Domain.Entities.Imigrantes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imigrei.Infra.Data.Configurations;

public class ImigranteAcoesConfiguration : IEntityTypeConfiguration<ImigranteAcoes>
{
    public void Configure(EntityTypeBuilder<ImigranteAcoes> builder)
    {
        builder.ToTable("imigrante_acoes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Acao).HasMaxLength(500).IsRequired();
        builder.Property(a => a.TipoAcao).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(a => a.DataRegistro).IsRequired();

        // O histórico é quase sempre lido por imigrante e em ordem cronológica.
        builder.HasIndex(a => new { a.ImigranteId, a.DataRegistro });

        builder.HasOne(a => a.Imigrante)
               .WithMany(i => i.Acoes)
               .HasForeignKey(a => a.ImigranteId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
