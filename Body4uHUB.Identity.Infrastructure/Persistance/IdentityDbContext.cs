using Body4uHUB.Identity.Domain.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Persistance
{
    internal sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();

            base.OnModelCreating(modelBuilder);
        }
    }
}
