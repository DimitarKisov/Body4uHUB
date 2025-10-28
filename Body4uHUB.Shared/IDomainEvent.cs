namespace Body4uHUB.Shared
{
    using MediatR;

    public interface IDomainEvent : INotification
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}
