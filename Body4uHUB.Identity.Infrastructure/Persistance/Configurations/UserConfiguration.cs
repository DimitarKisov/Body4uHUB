using Body4uHUB.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Body4uHUB.Identity.Infrastructure.Persistance.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(100)
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

            // Complex Type за ContactInfo
            builder.OwnsOne(x => x.ContactInfo, contact =>
            {
                contact.Property(y => y.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(200)
                    .IsRequired();

                contact.Property(y => y.PhoneNumber)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20)
                    .IsRequired();

                // Index
                contact.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Members_Email");
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
