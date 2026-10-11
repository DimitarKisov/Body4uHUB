using Body4uHUB.Services.Domain.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Services.Infrastructure.Persistence
{
    internal class ServicesDbContext : DbContext
    {
        public ServicesDbContext(DbContextOptions<ServicesDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrainerProfile> TrainerProfiles { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServicesDbContext).Assembly);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();

            base.OnModelCreating(modelBuilder);
        }
    }
}
