namespace Body4uHUB.Content.Infrastructure.Persistence
{
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Shared;
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified &&
                            x.Entity is Entity<Guid>);

            foreach (var entry in modifiedEntries)
            {
                var entity = (Entity<Guid>)entry.Entity;
                entity.SetModifiedAt();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}