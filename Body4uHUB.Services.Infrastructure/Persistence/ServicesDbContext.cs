using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Services.Infrastructure.Persistence.Converters;
using Body4uHUB.Shared.Domain;
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
        public DbSet<ServiceOffering> ServiceOfferings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServicesDbContext).Assembly);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<ServiceOfferingId>()
                .HaveConversion<ServiceOfferingConverter>();

            configurationBuilder
                .Properties<ServiceOrderId>()
                .HaveConversion<ServiceOrderConverter>();

            configurationBuilder
                .Properties<ReviewId>()
                .HaveConversion<ReviewConverter>();

            base.ConfigureConventions(configurationBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified &&
                            x.Entity is IModifiableEntity);

            foreach (var entry in modifiedEntries)
            {
                var entity = (IModifiableEntity)entry.Entity;
                entity.SetModifiedAt();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
