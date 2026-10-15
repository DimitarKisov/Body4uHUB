using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Identity.Domain.Events
{
    /// <summary>
    /// An unconfirmed user got a new email confirmation token.
    /// </summary>
    public sealed record EmailConfirmationTokenRegeneratedDomainEvent(Guid UserId) : DomainEvent;
}
