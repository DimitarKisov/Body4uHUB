using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations;

internal sealed class ContentViewConfiguration : IEntityTypeConfiguration<ContentView>
{
    public void Configure(EntityTypeBuilder<ContentView> builder)
    {
        builder.ToTable("ContentViews");

        builder.HasKey(x => new { x.TargetType, x.TargetId, x.VisitorHash, x.ViewDate });

        builder.Property(x => x.TargetType)
            .HasConversion<byte>();

        builder.Property(x => x.VisitorHash)
            .HasMaxLength(32)
            .IsFixedLength();

        // For the cleanup of previous days.
        builder.HasIndex(x => x.ViewDate);
    }
}
