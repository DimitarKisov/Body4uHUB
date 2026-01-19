using Body4uHUB.Shared.Domain.Abstractions;
namespace Body4uHUB.Shared.Domain.Base
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
        where TId : notnull
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        protected AggregateRoot(TId id)
            : base(id)
        {
        }

        protected AggregateRoot()
            : base(default!)
        {
        }

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
