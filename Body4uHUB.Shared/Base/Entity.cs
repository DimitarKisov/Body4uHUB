using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Shared.Domain.Base
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>>, ITimestampedEntity
        where TId : notnull
    {
        public TId Id { get; protected set; }

        // Set by the persistence layer on save (TimestampsInterceptor), so they stay default until then.
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        protected Entity(TId id)
        {
            Id = id;
        }

        // За EF Core
        protected Entity() { }

        // Explicit, so domain and application code cannot change the timestamps.
        void ITimestampedEntity.SetCreatedAt(DateTime utcNow)
        {
            CreatedAt = utcNow;
        }

        void ITimestampedEntity.SetModifiedAt(DateTime utcNow)
        {
            ModifiedAt = utcNow;
        }

        public bool Equals(Entity<TId> other)
        {
            return other is not null && Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is Entity<TId> entity && Equals(entity);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !Equals(left, right);
        }
    }
}
