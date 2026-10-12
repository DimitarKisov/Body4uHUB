using Body4uHUB.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Infrastructure.Persistance.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(MaxNameLength)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(MaxNameLength)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            builder.Property(x => x.IsEmailConfirmed)
                .IsRequired();

            builder.Property(x => x.EmailConfirmationToken)
                .IsRequired(false)
                .HasMaxLength(36);

            // Complex Type за ContactInfo
            builder.OwnsOne(x => x.ContactInfo, contact =>
            {
                contact.Property(y => y.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(MaxEmailLength)
                    .IsRequired();

                contact.Property(y => y.PhoneNumber)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(MaxPhoneNumberLength);

                // Index
                contact.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
            });

            var rolesNavigation = builder.Metadata.FindNavigation(nameof(User.Roles))!;
            rolesNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            // Конфигурация на Many-to-Many релацията
            builder.HasMany(u => u.Roles)
                .WithMany()
                .UsingEntity(
                    "UserRoles",
                    x => x.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"),
                    y => y.HasOne(typeof(User)).WithMany().HasForeignKey("UserId"),
                    z => z.HasKey("UserId", "RoleId"));

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}
