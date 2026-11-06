using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.UserId)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.SessionId)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedAtUtc)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.ExpiresAtUtc)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.RevokedAtUtc);

        builder.HasIndex(refreshToken => refreshToken.TokenHash)
            .IsUnique();

        builder.HasIndex(refreshToken => new { refreshToken.UserId, refreshToken.SessionId });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
