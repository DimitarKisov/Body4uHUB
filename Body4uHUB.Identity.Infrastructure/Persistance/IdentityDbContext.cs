using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Shared.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Persistance
{
    internal class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
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
