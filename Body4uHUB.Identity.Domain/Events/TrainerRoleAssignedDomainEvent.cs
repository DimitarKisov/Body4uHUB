using Body4uHUB.Shared.Domain.Base;

namespace Body4uHUB.Identity.Domain.Events;

/// <summary>
/// The user became a trainer.
/// </summary>
public sealed record TrainerRoleAssignedDomainEvent(Guid UserId) : DomainEvent;
