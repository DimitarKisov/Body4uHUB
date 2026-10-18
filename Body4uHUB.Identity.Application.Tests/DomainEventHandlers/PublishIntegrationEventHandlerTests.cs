using Body4uHUB.Identity.Application.DomainEventHandlers;
using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Shared.Application.Events;
using Moq;

namespace Body4uHUB.Identity.Application.Tests.DomainEventHandlers
{
    [TestFixture]
    public class PublishIntegrationEventHandlerTests
    {
        private readonly Guid _userId = Guid.NewGuid();

        private Mock<IEventBus> _eventBus;
        private PublishIntegrationEventHandler _handler;

        [SetUp]
        public void Setup()
        {
            _eventBus = new Mock<IEventBus>();
            _handler = new PublishIntegrationEventHandler(_eventBus.Object);
        }

        [Test]
        public async Task Handle_ShouldPublishUserRegisteredEvent_ForUserRegisteredDomainEvent()
        {
            await _handler.Handle(new UserRegisteredDomainEvent(_userId), CancellationToken.None);

            _eventBus.Verify(
                x => x.PublishAsync(It.Is<UserRegisteredEvent>(e => e.UserId == _userId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldPublishEmailConfirmationResendRequestedEvent_ForTokenRegeneratedDomainEvent()
        {
            await _handler.Handle(new EmailConfirmationTokenRegeneratedDomainEvent(_userId), CancellationToken.None);

            _eventBus.Verify(
                x => x.PublishAsync(It.Is<EmailConfirmationResendRequestedEvent>(e => e.UserId == _userId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldPublishTrainerAccountCreatedEvent_ForTrainerRoleAssignedDomainEvent()
        {
            await _handler.Handle(new TrainerRoleAssignedDomainEvent(_userId), CancellationToken.None);

            _eventBus.Verify(
                x => x.PublishAsync(It.Is<TrainerAccountCreatedEvent>(e => e.UserId == _userId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldPublishTrainerAccountDeletedEvent_ForTrainerRoleRevokedDomainEvent()
        {
            await _handler.Handle(new TrainerRoleRevokedDomainEvent(_userId), CancellationToken.None);

            _eventBus.Verify(
                x => x.PublishAsync(It.Is<TrainerAccountDeletedEvent>(e => e.UserId == _userId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldPublishPasswordResetRequestedEventWithToken_ForPasswordResetRequestedDomainEvent()
        {
            await _handler.Handle(new PasswordResetRequestedDomainEvent(_userId, "raw-token"), CancellationToken.None);

            _eventBus.Verify(
                x => x.PublishAsync(It.Is<PasswordResetRequestedEvent>(e => e.UserId == _userId && e.Token == "raw-token"), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
