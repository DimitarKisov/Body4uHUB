using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Identity.Domain.Events;

/// <summary>
/// A user asked to reset their password. Carries the raw token, because only its hash is stored on the user;
/// the email link is the only other place the raw token ever exists.
/// </summary>
public sealed record PasswordResetRequestedDomainEvent(Guid UserId, string Token) : DomainEvent;
