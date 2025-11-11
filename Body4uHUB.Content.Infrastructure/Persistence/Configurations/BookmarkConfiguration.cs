namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations
{
    using Body4uHUB.Content.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.ToTable("Bookmarks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.ArticleId)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Unique constraint
            builder.HasIndex(x => new { x.UserId, x.ArticleId })
                .IsUnique();

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ArticleId);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}