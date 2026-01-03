using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;

namespace Body4uHUB.Services.Infrastructure.Persistence.Configurations
{
    internal class ServiceOfferingConfiguration : IEntityTypeConfiguration<ServiceOffering>
    {
        public void Configure(EntityTypeBuilder<ServiceOffering> builder)
        {
            builder.ToTable("ServiceOfferings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .IsRequired();

            builder.Property<Guid>("TrainerProfileId")
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.MaxParticipants)
                .IsRequired();

            builder.Property(x => x.IsOnline)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.AverageRating)
                .IsRequired()
                .HasColumnType("decimal(3,2)")
                .HasDefaultValue(0);

            builder.OwnsOne(x => x.Price, price =>
            {
                price.Property(y => y.Amount)
                    .IsRequired()
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)");
                    
                price.Property(y => y.Currency)
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(x => x.Category)
                .HasConversion(
                    y => y.Id,
                    y => Enumeration.FromValue<ServiceCategory>(y))
                .IsRequired();

            builder.OwnsMany(x => x.Reviews, review =>
            {
                review.ToTable("Reviews");
                review.WithOwner().HasForeignKey("ServiceOfferingId");

                review.HasKey(x => x.Id);

                review.Property(x => x.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn();

                review.Property(x => x.ClientId)
                    .IsRequired();

                review.Property(x => x.Rating)
                    .IsRequired();

                review.Property(x => x.Comment)
                    .IsRequired()
                    .HasMaxLength(MaxCommentLength);

                review.Property(x => x.CreatedAt)
                    .IsRequired();

                review.HasIndex(x => x.OrderId)
                    .IsUnique()
                    .HasDatabaseName("IX_Reviews_ServiceOrderId");

                review.HasIndex(x => x.ClientId)
                    .HasDatabaseName("IX_Reviews_ClientId");
            });

            builder.HasIndex("TrainerProfileId");

            builder.HasIndex(x => x.Category);

            builder.HasIndex(x => x.IsActive);
        }
    }
}
