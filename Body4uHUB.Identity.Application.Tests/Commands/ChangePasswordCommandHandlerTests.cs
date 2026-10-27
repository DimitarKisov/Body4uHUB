using Body4uHUB.Identity.Application.Commands.ChangePassword;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Identity.Application.Tests.Fakes;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ChangePasswordCommandHandlerTests
    {
        private const string CurrentPassword = "Password1!";
        private const string NewPassword = "NewPassword1!";
        private const string CurrentHash = "AQAAAAEAACcQAAAAEDummyHashValue==";
        private const string NewHash = "AQAAAAEAACcQAAAAENewDummyHashValue==";

        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private User _user;

        private ChangePasswordCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _refreshTokenRepository = Sessions.EmptyRepository();
            _user = User.Create(CurrentHash, "Test", "User", "test@mail.com", "0884787878", DateTime.UtcNow);

            _userRepository
                .Setup(x => x.GetByIdAsync(_user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _passwordHasherService
                .Setup(x => x.VerifyPassword(CurrentPassword, CurrentHash))
                .Returns(true);

            _passwordHasherService
                .Setup(x => x.HashPassword(NewPassword))
                .Returns(NewHash);

            _handler = new ChangePasswordCommandHandler(
                _userRepository.Object, _passwordHasherService.Object, _refreshTokenRepository.Object, _unitOfWork.Object, TimeProvider.System);
        }

        [Test]
        public async Task Handle_ShouldReplacePasswordHashAndSave_WhenCurrentPasswordIsCorrect()
        {
            var result = await _handler.Handle(new ChangePasswordCommand(_user.Id, CurrentPassword, NewPassword), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.PasswordHash, Is.EqualTo(NewHash));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldEndOtherSessionsButKeepTheCurrentOne()
        {
            var current = RefreshToken.Issue(_user.Id, false, DateTime.UtcNow, TimeSpan.FromDays(7), TimeSpan.FromDays(30), out var currentRaw);
            var other = Sessions.Active(_user.Id);
            _refreshTokenRepository.SetupActive(_user.Id, current, other);

            var result = await _handler.Handle(new ChangePasswordCommand(_user.Id, CurrentPassword, NewPassword, currentRaw), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(current.RevokedAt, Is.Null);
            Assert.That(other.RevokedAt, Is.Not.Null);
        }

        [Test]
        public async Task Handle_ShouldEndEverySession_WhenThereIsNoCurrentSessionCookie()
        {
            var sessions = new[] { Sessions.Active(_user.Id), Sessions.Active(_user.Id) };
            _refreshTokenRepository.SetupActive(_user.Id, sessions);

            await _handler.Handle(new ChangePasswordCommand(_user.Id, CurrentPassword, NewPassword), CancellationToken.None);

            Assert.That(sessions.Select(x => x.RevokedAt), Has.All.Not.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnBusinessRuleViolationAndKeepPassword_WhenCurrentPasswordIsWrong()
        {
            var result = await _handler.Handle(new ChangePasswordCommand(_user.Id, "WrongPassword1!", NewPassword), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.BusinessRule));
            Assert.That(result.Error, Is.EqualTo(CurrentPasswordInvalid));
            Assert.That(_user.PasswordHash, Is.EqualTo(CurrentHash));
            _passwordHasherService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var result = await _handler.Handle(new ChangePasswordCommand(Guid.NewGuid(), CurrentPassword, NewPassword), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
            Assert.That(result.Error, Is.EqualTo(UserNotFound));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
