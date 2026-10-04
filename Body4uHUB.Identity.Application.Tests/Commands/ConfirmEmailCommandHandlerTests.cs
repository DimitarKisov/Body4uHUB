using Body4uHUB.Identity.Application.Commands.ConfirmEmail;
using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ConfirmEmailCommandHandlerTests
    {
        private const string ValidEmail = "test@mail.com";

        private Mock<IUserRepository> _userRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private User _user;

        private ConfirmEmailCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", ValidEmail, "0884787878");

            _handler = new ConfirmEmailCommandHandler(_userRepository.Object, _unitOfWork.Object);
        }

        [Test]
        public async Task Handle_ShouldConfirmEmailAndSave_WhenTokenIsValid()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            var result = await _handler.Handle(new ConfirmEmailCommand(ValidEmail, _user.EmailConfirmationToken), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.IsEmailConfirmed, Is.True);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrowSameErrorAsInvalidToken_WhenUserDoesNotExist()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var ex = Assert.ThrowsAsync<InvalidUserException>(() =>
                _handler.Handle(new ConfirmEmailCommand(ValidEmail, "any-token"), CancellationToken.None));

            Assert.That(ex.Error, Is.EqualTo(EmailConfirmationTokenInvalid));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Handle_ShouldThrowAndNotSave_WhenTokenIsInvalid()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            Assert.ThrowsAsync<InvalidUserException>(() =>
                _handler.Handle(new ConfirmEmailCommand(ValidEmail, "wrong-token"), CancellationToken.None));

            Assert.That(_user.IsEmailConfirmed, Is.False);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
