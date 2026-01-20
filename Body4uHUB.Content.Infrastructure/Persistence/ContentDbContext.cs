namespace Body4uHUB.Content.Infrastructure.Persistence
{
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Content.Infrastructure.Persistence.Converters;
    using Body4uHUB.Shared;
    using Body4uHUB.Shared.Domain.Abstractions;
    using Microsoft.EntityFrameworkCore;

    internal class ContentDbContext : DbContext
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        //THOSE THINGS ARE NOT USED ANYMORE. IN THIS PROJECT WE USE VALUE OBJECTS WITHOUT CONVERTERS. KEEPING THIS CODE COMMENTED FOR FUTURE CHANGES IF NEEDED.
        //protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        //{
        //    configurationBuilder
        //        .Properties<ArticleId>()
        //        .HaveConversion<ArticleIdConverter>();

        //    configurationBuilder
        //        .Properties<CommentId>()
        //        .HaveConversion<CommentIdConverter>();

        //    base.ConfigureConventions(configurationBuilder);
        //}

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified &&
                            x.Entity is IModifiableEntity);

            foreach (var entry in modifiedEntries)
            {
                var entity = (IModifiableEntity)entry.Entity;
                entity.SetModifiedAt();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}