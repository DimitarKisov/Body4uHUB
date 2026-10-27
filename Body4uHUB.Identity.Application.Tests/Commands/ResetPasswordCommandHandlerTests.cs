using Body4uHUB.Identity.Application.Commands.ResetPassword;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Identity.Application.Tests.Fakes;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ResetPasswordCommandHandlerTests
    {
        private const string ValidEmail = "test@mail.com";
        private const string CurrentHash = "AQAAAAEAACcQAAAAEDummyHashValue==";
        private const string NewPassword = "NewPassword1!";
        private const string NewHash = "AQAAAAEAACcQAAAAENewDummyHashValue==";

        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private User _user;
        private string _token;

        private ResetPasswordCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _refreshTokenRepository = Sessions.EmptyRepository();

            _user = User.Create(CurrentHash, "Test", "User", ValidEmail, "0884787878", DateTime.UtcNow);
            _user.ClearDomainEvents();
            _user.RequestPasswordReset(DateTime.UtcNow);
            _token = ((PasswordResetRequestedDomainEvent)_user.DomainEvents[0]).Token;

            _passwordHasherService.Setup(x => x.HashPassword(NewPassword)).Returns(NewHash);

            _handler = new ResetPasswordCommandHandler(
                _userRepository.Object, _passwordHasherService.Object, _refreshTokenRepository.Object, _unitOfWork.Object, TimeProvider.System);
        }

        [Test]
        public async Task Handle_ShouldSetNewPasswordAndSave_WhenTokenIsValid()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            var result = await _handler.Handle(new ResetPasswordCommand(ValidEmail, _token, NewPassword), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.PasswordHash, Is.EqualTo(NewHash));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldEndEverySession_WhenPasswordIsReset()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
            var sessions = new[] { Sessions.Active(_user.Id), Sessions.Active(_user.Id) };
            _refreshTokenRepository.SetupActive(_user.Id, sessions);

            var result = await _handler.Handle(new ResetPasswordCommand(ValidEmail, _token, NewPassword), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(sessions.Select(x => x.RevokedAt), Has.All.Not.Null);
        }

        [Test]
        public void Handle_ShouldThrowAndKeepPassword_WhenTokenIsWrong()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            var ex = Assert.ThrowsAsync<InvalidUserException>(() => _handler.Handle(new ResetPasswordCommand(ValidEmail, "wrong-token", NewPassword), CancellationToken.None));

            Assert.That(ex.Error, Is.EqualTo(PasswordResetTokenInvalid));
            Assert.That(_user.PasswordHash, Is.EqualTo(CurrentHash));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Handle_ShouldThrowTheSameError_WhenUserDoesNotExist()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var ex = Assert.ThrowsAsync<InvalidUserException>(() => _handler.Handle(new ResetPasswordCommand(ValidEmail, _token, NewPassword), CancellationToken.None));

            Assert.That(ex.Error, Is.EqualTo(PasswordResetTokenInvalid));
            // The new password is hashed either way, so a missing user is not faster to detect.
            _passwordHasherService.Verify(x => x.HashPassword(NewPassword), Times.Once);
        }
    }
}
