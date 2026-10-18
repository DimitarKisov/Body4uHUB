namespace Body4uHUB.Shared.Application.Events
{
    /// <summary>
    /// Carries the raw reset token to the email consumer (the user stores only its hash).
    /// The outbox keeps the message until it is delivered; the token is single-use and expires in an hour.
    /// </summary>
    public class PasswordResetRequestedEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
