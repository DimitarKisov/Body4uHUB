namespace Body4uHUB.Shared.Application.Events
{
    public class EmailConfirmationResendRequestedEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
    }
}
