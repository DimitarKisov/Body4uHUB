namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations
{
    using Body4uHUB.Content.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class ForumPostConfiguration : IEntityTypeConfiguration<ForumPost>
    {
        public void Configure(EntityTypeBuilder<ForumPost> builder)
        {
            builder.ToTable("ForumPosts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content)
                .IsRequired();

            builder.Property(x => x.AuthorId)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            builder.HasIndex(x => x.AuthorId);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}