using Body4uHUB.Identity.Domain.Models;
using MassTransit;
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

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();

            base.OnModelCreating(modelBuilder);
        }
    }
}
