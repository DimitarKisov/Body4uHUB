using Body4uHUB.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RefreshTokenConstants;

namespace Body4uHUB.Identity.Infrastructure.Persistance.Configurations
{
    internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .HasMaxLength(TokenHashLength)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.FamilyExpiresAt)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            // Lookup on every refresh; unique, so a hash can never point to two sessions.
            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.FamilyId);

            // Tokens go with their user; references by Id only, like the other aggregates.
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(x => x.WasRotated);
            builder.Ignore(x => x.DomainEvents);
        }
    }
}
