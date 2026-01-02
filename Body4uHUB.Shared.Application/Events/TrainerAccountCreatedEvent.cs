namespace Body4uHUB.Shared.Application.Events
{
    public class TrainerAccountCreatedEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
        public string Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
