using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Identity.Domain.Events;

/// <summary>
/// A new user registered and must confirm their email.
/// </summary>
public sealed record UserRegisteredDomainEvent(Guid UserId) : DomainEvent;
