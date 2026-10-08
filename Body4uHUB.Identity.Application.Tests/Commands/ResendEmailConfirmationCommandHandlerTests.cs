using Body4uHUB.Identity.Application.Commands.ResendEmailConfirmation;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ResendEmailConfirmationCommandHandlerTests
    {
        private const string ValidEmail = "test@mail.com";

        private Mock<IUserRepository> _userRepository;
        private Mock<IEventBus> _eventBus;
        private Mock<IUnitOfWork> _unitOfWork;
        private User _user;

        private ResendEmailConfirmationCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _eventBus = new Mock<IEventBus>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", ValidEmail, "0884787878");

            _handler = new ResendEmailConfirmationCommandHandler(_userRepository.Object, _eventBus.Object, _unitOfWork.Object);
        }

        [Test]
        public async Task Handle_ShouldRegenerateTokenAndPublishEventBeforeSaving_WhenEmailIsNotConfirmed()
        {
            var oldToken = _user.EmailConfirmationToken;
            var calls = new List<string>();

            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _eventBus
                .Setup(x => x.PublishAsync(It.IsAny<EmailConfirmationResendRequestedEvent>()))
                .Callback(() => calls.Add("publish"))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => calls.Add("save"));

            var result = await _handler.Handle(new ResendEmailConfirmationCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.EmailConfirmationToken, Is.Not.EqualTo(oldToken));

            // The outbox only stores the message if it is published before SaveChanges.
            Assert.That(calls, Is.EqualTo(new[] { "publish", "save" }));
            _eventBus.Verify(
                x => x.PublishAsync(It.Is<EmailConfirmationResendRequestedEvent>(e => e.UserId == _user.Id)),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldSucceedWithoutPublishing_WhenUserDoesNotExist()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await _handler.Handle(new ResendEmailConfirmationCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            _eventBus.Verify(x => x.PublishAsync(It.IsAny<EmailConfirmationResendRequestedEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldSucceedWithoutPublishing_WhenEmailIsAlreadyConfirmed()
        {
            var token = _user.EmailConfirmationToken;
            _user.ConfirmEmail(token);

            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            var result = await _handler.Handle(new ResendEmailConfirmationCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.EmailConfirmationToken, Is.EqualTo(token));
            _eventBus.Verify(x => x.PublishAsync(It.IsAny<EmailConfirmationResendRequestedEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
