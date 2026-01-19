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

            builder.OwnsMany(b => b.Posts, postBuilder =>
            {
                postBuilder.ToTable("ForumPosts");

                postBuilder.HasKey(x => x.Id);

                postBuilder.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

                postBuilder.WithOwner()
                    .HasForeignKey("ForumTopicId");

                postBuilder.Property(x => x.Content)
                    .IsRequired();

                postBuilder.Property(x => x.AuthorId)
                    .IsRequired();

                postBuilder.Property(x => x.IsDeleted)
                    .IsRequired();

                postBuilder.Property(x => x.CreatedAt)
                    .IsRequired();

                postBuilder.Property(x => x.ModifiedAt)
                    .IsRequired(false);

                postBuilder.HasIndex(x => x.AuthorId);
            });

            // Indexes
            builder.HasIndex(x => x.AuthorId);
            builder.HasIndex(x => x.CreatedAt);

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);
        }
    }
}