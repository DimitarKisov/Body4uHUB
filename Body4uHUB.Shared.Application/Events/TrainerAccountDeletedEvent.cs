namespace Body4uHUB.Shared.Application.Events
{
    public class TrainerAccountDeletedEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
    }
}
