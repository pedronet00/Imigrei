using Imigrei.Domain.Entities.Identidade;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imigrei.Infra.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UsuarioId).IsRequired();

        // SHA-256 em hexadecimal: sempre 64 caracteres ASCII.
        builder.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
        builder.Property(t => t.SubstituidoPorTokenHash).HasMaxLength(64);

        builder.Property(t => t.CriadoEm).IsRequired();
        builder.Property(t => t.ExpiraEm).IsRequired();
        builder.Property(t => t.CriadoPorIp).HasMaxLength(45);

        builder.HasIndex(t => t.TokenHash).IsUnique();
        builder.HasIndex(t => t.UsuarioId);

        // Propriedades calculadas do domínio não têm coluna.
        builder.Ignore(t => t.Ativo);
        builder.Ignore(t => t.Expirado);
        builder.Ignore(t => t.Revogado);
    }
}
