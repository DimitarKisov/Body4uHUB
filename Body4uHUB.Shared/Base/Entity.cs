using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Shared.Domain.Base
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>>, IModifiableEntity
        where TId : notnull
    {
        public TId Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? ModifiedAt { get; protected set; }

        protected Entity(TId id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        // За EF Core
        protected Entity() { }

        public void SetModifiedAt()
        {
            ModifiedAt = DateTime.UtcNow;
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
