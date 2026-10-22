using Body4uHUB.Shared.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Body4uHUB.Shared.Infrastructure.Persistence
{
    /// <summary>
    /// Sets CreatedAt on added entities, and ModifiedAt on modified entities and on their owners.
    /// A change inside an aggregate (owned entity added/edited/removed, many-to-many link added/removed)
    /// leaves the owner Unchanged in EF, so the change is propagated up to the owner here.
    /// </summary>
    public sealed class TimestampsInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
    {
        private readonly TimeProvider _timeProvider = timeProvider;

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            SetTimestamps(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetTimestamps(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void SetTimestamps(DbContext context)
        {
            if (context is null)
            {
                return;
            }

            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
            var entries = context.ChangeTracker.Entries().ToList();
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

            var changedEntries = entries
                .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntries)
            {
                if (entry.State == EntityState.Added && entry.Entity is ITimestampedEntity addedEntity)
                {
                    addedEntity.SetCreatedAt(utcNow);
                }

                if (entry.State == EntityState.Modified || entry.Metadata.IsOwned())
                {
                    MarkModified(entry, entries, visited, utcNow);
                }

                // Many-to-many join entries (e.g. UserRoles) point to the entity that declares the
                // collection. The shadow inverse of a unidirectional many-to-many (Role -> Users) is skipped.
                foreach (var foreignKey in entry.Metadata.GetForeignKeys()
                    .Where(x => x.GetReferencingSkipNavigations().Any(y => !y.IsShadowProperty())))
                {
                    var principal = FindPrincipal(entry, foreignKey, entries);
                    if (principal is not null)
                    {
                        MarkModified(principal, entries, visited, utcNow);
                    }
                }
            }
        }

        private static void MarkModified(EntityEntry entry, List<EntityEntry> entries, HashSet<object> visited, DateTime utcNow)
        {
            if (!visited.Add(entry.Entity))
            {
                return;
            }

            if (entry.State is EntityState.Unchanged or EntityState.Modified &&
                entry.Entity is ITimestampedEntity modifiedEntity)
            {
                modifiedEntity.SetModifiedAt(utcNow);
            }

            var ownership = entry.Metadata.FindOwnership();
            if (ownership is null)
            {
                return;
            }

            var owner = FindPrincipal(entry, ownership, entries);
            if (owner is not null)
            {
                MarkModified(owner, entries, visited, utcNow);
            }
        }

        private static EntityEntry FindPrincipal(EntityEntry dependent, IForeignKey foreignKey, List<EntityEntry> entries)
        {
            var keyValues = foreignKey.Properties
                .Select(x => dependent.State == EntityState.Deleted
                    ? dependent.Property(x.Name).OriginalValue
                    : dependent.Property(x.Name).CurrentValue)
                .ToList();

            return entries.FirstOrDefault(x =>
                foreignKey.PrincipalEntityType.IsAssignableFrom(x.Metadata) &&
                foreignKey.PrincipalKey.Properties
                    .Select(y => x.Property(y.Name).CurrentValue)
                    .SequenceEqual(keyValues));
        }
    }
}
