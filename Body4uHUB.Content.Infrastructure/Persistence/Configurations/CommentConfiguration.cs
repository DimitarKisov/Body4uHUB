namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations
{
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .IsRequired();

            builder.Property(x => x.Content)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.AuthorId)
                .IsRequired();

            builder.Property(x => x.ArticleId)
                .IsRequired();

            builder.Property(x => x.ParentCommentId)
                .IsRequired(false);

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(x => x.ArticleId);
            builder.HasIndex(x => x.AuthorId);
            builder.HasIndex(x => x.ParentCommentId);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}