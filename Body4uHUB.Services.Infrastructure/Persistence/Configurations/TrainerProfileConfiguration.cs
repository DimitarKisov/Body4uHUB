using Body4uHUB.Services.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

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

            builder.HasMany<ServiceOffering>("_services")
                .WithOne()
                .HasForeignKey("TrainerProfileId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasIndex(x => x.IsActive);

            builder.HasIndex(x => x.AverageRating);
        }
    }
}
