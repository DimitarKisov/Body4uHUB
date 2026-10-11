using Body4uHUB.Shared.Application.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Identity.Infrastructure.Messaging
{
    internal sealed class EventBus(
        IPublishEndpoint publishEndpoint,
        ILogger<EventBus> logger)
        : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly ILogger<EventBus> _logger = logger;

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IntegrationEvent
        {
            // With UseBusOutbox() this only adds the message to the DbContext. It is stored by SaveChanges
            // and sent to RabbitMQ later by the outbox delivery service.
            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogInformation(
                "Event {EventType} {EventId} added to the outbox",
                @event.EventType,
                @event.EventId);
        }
    }
}
