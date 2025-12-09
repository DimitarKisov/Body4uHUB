namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations
{
    using Body4uHUB.Content.Domain.Enumerations;
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Shared;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            // Comments relationship
            builder.HasMany(x => x.Comments)
                .WithOne()
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.AuthorId);
            builder.HasIndex(x => x.PublishedAt);
            builder.HasIndex(x => x.Status);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}