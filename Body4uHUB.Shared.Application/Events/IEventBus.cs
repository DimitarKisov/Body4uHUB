namespace Body4uHUB.Shared.Application.Events
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IntegrationEvent;
    }
}
