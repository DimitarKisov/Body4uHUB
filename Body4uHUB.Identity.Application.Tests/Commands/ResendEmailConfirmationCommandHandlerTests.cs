using Body4uHUB.Identity.Application.Commands.ResendEmailConfirmation;
using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ResendEmailConfirmationCommandHandlerTests
    {
        private const string ValidEmail = "test@mail.com";

        private Mock<IUserRepository> _userRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private User _user;

        private ResendEmailConfirmationCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", ValidEmail, "0884787878");
            _user.ClearDomainEvents();

            _handler = new ResendEmailConfirmationCommandHandler(_userRepository.Object, _unitOfWork.Object);
        }

        [Test]
        public async Task Handle_ShouldRegenerateTokenAndRaiseEvent_WhenEmailIsNotConfirmed()
        {
            var oldToken = _user.EmailConfirmationToken;

            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            var result = await _handler.Handle(new ResendEmailConfirmationCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.EmailConfirmationToken, Is.Not.EqualTo(oldToken));
            Assert.That(_user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<EmailConfirmationTokenRegeneratedDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldSucceedWithoutRaisingEvent_WhenUserDoesNotExist()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await _handler.Handle(new ResendEmailConfirmationCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.DomainEvents, Is.Empty);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldSucceedWithoutRaisingEvent_WhenEmailIsAlreadyConfirmed()
        {
            var token = _user.EmailConfirmationToken;
            _user.ConfirmEmail(token);

            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            var result = await _handler.Handle(new ResendEmailConfirmationCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.EmailConfirmationToken, Is.EqualTo(token));
            Assert.That(_user.DomainEvents, Is.Empty);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
