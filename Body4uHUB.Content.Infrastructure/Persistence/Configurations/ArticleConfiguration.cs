using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static Body4uHUB.Content.Domain.Constants.ModelConstants;

namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations
{
    internal class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasMaxLength(ArticleConstants.ContentMaxLength)
                .IsRequired();

            builder.Property(x => x.AuthorId)
                .IsRequired();

            builder.Property(x => x.ViewCount)
                .IsRequired();

            builder.Property(x => x.PublishedAt)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            // ArticleStatus enumeration
            builder.Property(x => x.Status)
                .HasConversion(
                    v => v.Id,
                    v => Enumeration.FromValue<ArticleStatus>(v))
                .IsRequired();

            builder.OwnsMany(b => b.Comments, commentBuilder =>
            {
                commentBuilder.ToTable("Comments");

                commentBuilder.HasKey(c => c.Id);

                commentBuilder.Property(c => c.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn()
                    .IsRequired();

                commentBuilder.Property(c => c.Content)
                    .HasMaxLength(CommentConstants.ContentMaxLength)
                    .IsRequired();

                commentBuilder.Property(c => c.AuthorId)
                    .IsRequired();

                commentBuilder.WithOwner()
                    .HasForeignKey("ArticleId");

                commentBuilder.Property(c => c.ParentCommentId)
                    .IsRequired(false);

                commentBuilder.Property(c => c.IsDeleted)
                    .IsRequired();

                commentBuilder.Property(c => c.CreatedAt)
                    .IsRequired();

                commentBuilder.Property(c => c.ModifiedAt)
                    .IsRequired(false);

                // Indexes
                commentBuilder.HasIndex("ArticleId");
                commentBuilder.HasIndex(c => c.AuthorId);
                commentBuilder.HasIndex(c => c.ParentCommentId);
            });

            // Indexes
            builder.HasIndex(x => x.AuthorId);
            builder.HasIndex(x => x.PublishedAt);
            builder.HasIndex(x => x.Status);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}