using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;
using Body4uHUB.Shared.Domain.Enumerations;

namespace Body4uHUB.Services.Infrastructure.Persistence.Configurations
{
    internal class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
    {
        public void Configure(EntityTypeBuilder<ServiceOrder> builder)
        {
            builder.ToTable("ServiceOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .IsRequired();

            builder.Property(x => x.ClientId)
                .IsRequired();

            builder.Property(x => x.TrainerId)
                .IsRequired();

            builder.Property(x => x.ServiceOfferingId)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(NotesMaxLength);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion(
                    y => y.Id,
                    y => Enumeration.FromValue<OrderStatus>(y))
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .HasConversion(
                    y => y.Id,
                    y => Enumeration.FromValue<PaymentStatus>(y))
                .IsRequired();

            builder.OwnsOne(x => x.TotalPrice, price =>
            {
                price.Property(y => y.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(y => y.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(x => x.IsReviewed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => x.ClientId);

            builder.HasIndex(x => x.TrainerId);

            builder.HasIndex(x => x.Status);
        }
    }
}
