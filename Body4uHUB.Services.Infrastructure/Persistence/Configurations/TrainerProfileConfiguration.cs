using Body4uHUB.Services.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;
using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Shared;

namespace Body4uHUB.Services.Infrastructure.Persistence.Configurations
{
    internal class TrainerProfileConfiguration : IEntityTypeConfiguration<TrainerProfile>
    {
        public void Configure(EntityTypeBuilder<TrainerProfile> builder)
        {
            builder.ToTable("TrainerProfiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Bio)
                .HasMaxLength(BioMaxLength);

            builder.Property(x => x.YearsOfExperience)
                .IsRequired();

            builder.Property(x => x.AverageRating)
                .HasColumnType("decimal(3,2)")
                .IsRequired();

            builder.Property(x => x.TotalReviews)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Specializations - stored as JSON
            builder.Property<List<string>>("_specializations")
                .HasColumnName("Specializations")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)");

            // Certifications - stored as JSON
            builder.Property<List<string>>("_certifications")
                .HasColumnName("Certifications")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)");

            builder.OwnsMany(tp => tp.Services, serviceBuilder =>
            {
                serviceBuilder.ToTable("ServiceOfferings");

                serviceBuilder.WithOwner()
                    .HasForeignKey("TrainerProfileId");

                serviceBuilder.Property<Guid>("Id")
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                serviceBuilder.HasKey("Id");

                serviceBuilder.Property(s => s.Name)
                    .HasMaxLength(NameMaxLength)
                    .IsRequired();

                serviceBuilder.Property(s => s.Description)
                    .HasMaxLength(DescriptionMaxLength)
                    .IsRequired();

                serviceBuilder.Property(s => s.DurationInMinutes)
                    .IsRequired();

                serviceBuilder.Property(s => s.MaxParticipants)
                    .IsRequired();

                serviceBuilder.Property(s => s.IsOnline)
                    .IsRequired();

                serviceBuilder.Property(s => s.IsActive)
                    .IsRequired();

                serviceBuilder.Property(s => s.CreatedAt)
                    .IsRequired();

                serviceBuilder.Property(s => s.AverageRating)
                    .HasColumnType("decimal(3,2)")
                    .HasDefaultValue(0)
                    .IsRequired();

                serviceBuilder.OwnsOne(sb => sb.Price, price =>
                {
                    price.Property(p => p.Amount)
                        .IsRequired()
                        .HasColumnName("Price")
                        .HasColumnType("decimal(18,2)");

                    price.Property(p => p.Currency)
                        .HasMaxLength(3)
                        .IsRequired();
                });

                serviceBuilder.Property(s => s.Category)
                    .HasConversion(
                        y => y.Id,
                        y => Enumeration.FromValue<ServiceCategory>(y))
                    .IsRequired();

                serviceBuilder.OwnsMany(sb => sb.Reviews, review =>
                {
                    review.ToTable("Reviews");
                    review.WithOwner().HasForeignKey("ServiceOfferingId");

                    review.HasKey(r => r.Id);

                    review.Property(r => r.Id)
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .UseIdentityColumn();

                    review.Property(r => r.ClientId)
                        .IsRequired();

                    review.Property(r => r.Rating)
                        .IsRequired();

                    review.Property(r => r.Comment)
                        .IsRequired()
                        .HasMaxLength(MaxCommentLength);

                    review.Property(r => r.CreatedAt)
                        .IsRequired();

                    review.HasIndex(r => r.OrderId)
                        .IsUnique()
                        .HasDatabaseName("IX_Reviews_ServiceOrderId");

                    review.HasIndex(    r => r.ClientId)
                        .HasDatabaseName("IX_Reviews_ClientId");
                });

                serviceBuilder.HasIndex("TrainerProfileId");

                serviceBuilder.HasIndex(x => x.Category);

                serviceBuilder.HasIndex(x => x.IsActive);
            });

            // Indexes
            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasIndex(x => x.IsActive);

            builder.HasIndex(x => x.AverageRating);
        }
    }
}
