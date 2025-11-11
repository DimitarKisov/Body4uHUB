namespace Body4uHUB.Content.Infrastructure.Persistence.Configurations
{
    using Body4uHUB.Content.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class ForumTopicConfiguration : IEntityTypeConfiguration<ForumTopic>
    {
        public void Configure(EntityTypeBuilder<ForumTopic> builder)
        {
            builder.ToTable("ForumTopics");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.AuthorId)
                .IsRequired();

            builder.Property(x => x.IsLocked)
                .IsRequired();

            builder.Property(x => x.ViewCount)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired(false);

            // Posts relationship
            builder.HasMany(x => x.Posts)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.AuthorId);
            builder.HasIndex(x => x.CreatedAt);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}