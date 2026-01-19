using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Shared.Domain.Base
{
    public abstract record DomainEvent(Guid Id, DateTime OccurredOn) : IDomainEvent
    {
        protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
    }
}
