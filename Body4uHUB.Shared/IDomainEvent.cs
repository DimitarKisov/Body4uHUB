using MediatR;

namespace Body4uHUB.Shared
{
    public interface IDomainEvent : INotification
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}
