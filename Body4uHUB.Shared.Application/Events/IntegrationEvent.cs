namespace Body4uHUB.Shared.Application.Events
{
    public abstract class IntegrationEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public string EventType => GetType().Name;
    }
}
