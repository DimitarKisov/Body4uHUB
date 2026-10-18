using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Shared.Application.Events;
using MediatR;

namespace Body4uHUB.Identity.Application.DomainEventHandlers
{
    /// <summary>
    /// Turns the User domain events into integration events for the other services and consumers.
    /// The UnitOfWork dispatches domain events before SaveChanges, so the outbox stores
    /// these messages in the same transaction as the change that raised them.
    /// </summary>
    internal sealed class PublishIntegrationEventHandler(IEventBus eventBus)
        : INotificationHandler<UserRegisteredDomainEvent>,
          INotificationHandler<EmailConfirmationTokenRegeneratedDomainEvent>,
          INotificationHandler<TrainerRoleAssignedDomainEvent>,
          INotificationHandler<TrainerRoleRevokedDomainEvent>,
          INotificationHandler<PasswordResetRequestedDomainEvent>
    {
        private readonly IEventBus _eventBus = eventBus;

        public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
            => _eventBus.PublishAsync(new UserRegisteredEvent { UserId = notification.UserId }, cancellationToken);

        public Task Handle(EmailConfirmationTokenRegeneratedDomainEvent notification, CancellationToken cancellationToken)
            => _eventBus.PublishAsync(new EmailConfirmationResendRequestedEvent { UserId = notification.UserId }, cancellationToken);

        public Task Handle(TrainerRoleAssignedDomainEvent notification, CancellationToken cancellationToken)
            => _eventBus.PublishAsync(new TrainerAccountCreatedEvent { UserId = notification.UserId }, cancellationToken);

        public Task Handle(TrainerRoleRevokedDomainEvent notification, CancellationToken cancellationToken)
            => _eventBus.PublishAsync(new TrainerAccountDeletedEvent { UserId = notification.UserId }, cancellationToken);

        public Task Handle(PasswordResetRequestedDomainEvent notification, CancellationToken cancellationToken)
            => _eventBus.PublishAsync(new PasswordResetRequestedEvent { UserId = notification.UserId, Token = notification.Token }, cancellationToken);
    }
}
