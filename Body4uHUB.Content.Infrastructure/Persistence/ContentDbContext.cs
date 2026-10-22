using Body4uHUB.Content.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Persistence;

internal class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<ForumTopic> ForumTopics { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<ContentView> ContentViews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
