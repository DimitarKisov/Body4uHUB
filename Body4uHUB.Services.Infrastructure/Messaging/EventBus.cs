using Body4uHUB.Shared.Application.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Services.Infrastructure.Messaging
{
    internal class EventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<EventBus> _logger;

        public EventBus(
            IPublishEndpoint publishEndpoint,
            ILogger<EventBus> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IntegrationEvent
        {
            try
            {
                await _publishEndpoint.Publish(@event);

                _logger.LogInformation("Event published: {EventId} at {OccurredAt} of type {EventType}", @event.EventId, @event.OccurredAt, @event.EventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event: {EventId} of type {EventType}", @event.EventId, @event.EventType);
                throw;
            }
        }
    }
}
