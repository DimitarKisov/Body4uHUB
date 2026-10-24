namespace Body4uHUB.Shared.Application.Events
{
    public abstract class IntegrationEvent
    {
        // init, not get-only: the serializer skips get-only properties, so a consumer would get a new
        // EventId and its own "now" instead of the values the publisher sent.
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
        public string EventType => GetType().Name;
    }
}
