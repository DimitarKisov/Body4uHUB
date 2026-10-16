using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Identity.Domain.Events;

/// <summary>
/// The user is no longer a trainer.
/// </summary>
public sealed record TrainerRoleRevokedDomainEvent(Guid UserId) : DomainEvent;
