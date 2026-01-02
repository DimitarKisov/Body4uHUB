using Body4uHUB.Shared.Application.Events;
using MassTransit;

namespace Body4uHUB.Services.Infrastructure.Messaging
{
    internal class EventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EventBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IntegrationEvent
        {
            try
            {
                await _publishEndpoint.Publish(@event);

                //LOG INFo
            }
            catch (Exception ex)
            {
                //LOG INFO
            }
        }
    }
}
