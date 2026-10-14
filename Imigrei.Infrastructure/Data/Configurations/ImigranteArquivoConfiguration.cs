using Imigrei.Domain.Entities.Imigrantes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imigrei.Infra.Data.Configurations;

public class ImigranteArquivoConfiguration : IEntityTypeConfiguration<ImigranteArquivo>
{
    public void Configure(EntityTypeBuilder<ImigranteArquivo> builder)
    {
        builder.ToTable("imigrante_arquivos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.NomeOriginal).HasMaxLength(260).IsRequired();
        builder.Property(a => a.NomeArmazenado).HasMaxLength(100).IsRequired();
        builder.Property(a => a.CaminhoRelativo).HasMaxLength(300).IsRequired();
        builder.Property(a => a.ContentType).HasMaxLength(150).IsRequired();
        builder.Property(a => a.TamanhoBytes).IsRequired();

        // SHA-256 em hexadecimal: sempre 64 caracteres ASCII.
        builder.Property(a => a.HashSha256).HasMaxLength(64).IsRequired();

        builder.Property(a => a.TipoArquivo).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.DataUpload).IsRequired();

        builder.HasIndex(a => a.ImigranteId);
        builder.HasIndex(a => a.HashSha256);

        builder.HasOne(a => a.Imigrante)
               .WithMany(i => i.Arquivos)
               .HasForeignKey(a => a.ImigranteId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
