using Body4uHUB.Identity.Application.Commands.ForgotPassword;
using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ForgotPasswordCommandHandlerTests
    {
        private const string ValidEmail = "test@mail.com";

        private Mock<IUserRepository> _userRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private User _user;

        private ForgotPasswordCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", ValidEmail, "0884787878", DateTime.UtcNow);
            _user.ClearDomainEvents();

            _handler = new ForgotPasswordCommandHandler(_userRepository.Object, _unitOfWork.Object, TimeProvider.System);
        }

        [Test]
        public async Task Handle_ShouldStoreTokenHashAndRaiseEvent_WhenUserExists()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            var result = await _handler.Handle(new ForgotPasswordCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.PasswordResetTokenHash, Is.Not.Null);
            Assert.That(_user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<PasswordResetRequestedDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldSucceedWithoutSaving_WhenUserDoesNotExist()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await _handler.Handle(new ForgotPasswordCommand(ValidEmail), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
