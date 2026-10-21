using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Exceptions;
using Body4uHUB.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal sealed class UnitOfWork(IdentityDbContext dbContext, IPublisher publisher) : IUnitOfWork
    {
        private readonly IdentityDbContext _dbContext = dbContext;
        private readonly IPublisher _publisher = publisher;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatched before saving: the handlers publish integration events, which the bus outbox
            // only stages in the DbContext, so SaveChanges stores them in the same transaction as the change.
            await DispatchDomainEventsAsync(cancellationToken);

            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
            {
                throw new UniqueConstraintViolationException(ex);
            }
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var aggregates = _dbContext.ChangeTracker
                .Entries<IAggregateRoot>()
                .Select(x => x.Entity)
                .Where(x => x.DomainEvents.Count > 0)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(x => x.DomainEvents)
                .ToList();

            // Cleared before publishing, so a second SaveChanges in the same scope does not publish them again.
            aggregates.ForEach(x => x.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
