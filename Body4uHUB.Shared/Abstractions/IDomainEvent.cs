using MediatR;

namespace Body4uHUB.Shared.Domain.Abstractions
{
    public interface IDomainEvent : INotification
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}
