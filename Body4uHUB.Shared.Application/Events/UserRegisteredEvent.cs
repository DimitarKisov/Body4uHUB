namespace Body4uHUB.Shared.Application.Events
{
    public class UserRegisteredEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
    }
}
