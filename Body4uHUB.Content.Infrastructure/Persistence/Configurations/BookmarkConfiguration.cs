using Body4uHUB.Content.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations
{
    internal class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.ToTable("Bookmarks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.ArticleId)
                .IsRequired();

            builder.Property(x => x.ArticleNumber)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Unique constraint
            builder.HasIndex(x => new { x.UserId, x.ArticleId })
                .IsUnique();

            // Indexes
            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.ArticleId);

            builder.HasIndex(x => x.ArticleNumber);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}