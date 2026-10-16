using Body4uHUB.Identity.Application.Commands.Login;
using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Application.Tests.Fakes;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class LoginCommandHandlerTests
    {
        private const string ValidEmail = "test@mail.com";
        private const string ValidPassword = "Password1!";
        private const string PasswordHash = "AQAAAAEAACcQAAAAEDummyHashValue==";
        private const string AccessToken = "access-token";

        private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        private Mock<IUserRepository> _userRepository;
        private Mock<IRoleRepository> _roleRepository;
        private Mock<IJwtTokenService> _jwtTokenService;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<IUnitOfWork> _unitOfWork;
        private User _user;

        private LoginCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _roleRepository = new Mock<IRoleRepository>();
            _jwtTokenService = new Mock<IJwtTokenService>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _user = User.Create(PasswordHash, "Test", "User", ValidEmail, "0884787878", Now);
            _user.ConfirmEmail(_user.EmailConfirmationToken, Now);

            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _passwordHasherService
                .Setup(x => x.VerifyPassword(ValidPassword, PasswordHash))
                .Returns(true);

            _roleRepository
                .Setup(x => x.FindByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _jwtTokenService
                .Setup(x => x.GenerateAccessToken(_user.Id, ValidEmail, It.IsAny<IReadOnlyCollection<Role>>()))
                .Returns(AccessToken);

            _handler = new LoginCommandHandler(
                _userRepository.Object,
                _roleRepository.Object,
                _jwtTokenService.Object,
                _passwordHasherService.Object,
                _unitOfWork.Object,
                new FixedTimeProvider(new DateTimeOffset(Now.AddHours(1), TimeSpan.Zero)));
        }

        [Test]
        public async Task Handle_ShouldReturnTokenAndUpdateLastLogin_WhenCredentialsAreValid()
        {
            var role = Role.Create("Administrator");
            _user.AddRole(role);

            _roleRepository
                .Setup(x => x.FindByIdsAsync(
                    It.Is<IReadOnlyCollection<Guid>>(ids => ids.SequenceEqual(new[] { role.Id })),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([role]);

            var result = await _handler.Handle(new LoginCommand(ValidEmail, ValidPassword), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.AccessToken, Is.EqualTo(AccessToken));
            Assert.That(result.Value.User.Id, Is.EqualTo(_user.Id));
            Assert.That(result.Value.User.Roles.Select(x => x.Name), Is.EqualTo(new[] { "Administrator" }));
            Assert.That(_user.LastLoginAt, Is.EqualTo(Now.AddHours(1)));
            _jwtTokenService.Verify(
                x => x.GenerateAccessToken(_user.Id, ValidEmail, It.Is<IReadOnlyCollection<Role>>(roles => roles.Single() == role)),
                Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await _handler.Handle(new LoginCommand(ValidEmail, ValidPassword), CancellationToken.None);

            AssertRejected(result, ErrorType.Unauthorized, InvalidCredentials);
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorized_WhenPasswordIsWrong()
        {
            var result = await _handler.Handle(new LoginCommand(ValidEmail, "WrongPassword1!"), CancellationToken.None);

            AssertRejected(result, ErrorType.Unauthorized, InvalidCredentials);
            Assert.That(_user.LastLoginAt, Is.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnForbidden_WhenEmailIsNotConfirmed()
        {
            var unconfirmedUser = User.Create(PasswordHash, "Test", "User", ValidEmail, "0884787878", Now);

            _userRepository
                .Setup(x => x.GetByEmailAsync(ValidEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(unconfirmedUser);

            var result = await _handler.Handle(new LoginCommand(ValidEmail, ValidPassword), CancellationToken.None);

            AssertRejected(result, ErrorType.Forbidden, EmailNotConfirmed);
            Assert.That(unconfirmedUser.LastLoginAt, Is.Null);
        }

        private void AssertRejected(Result<AuthResponseDto> result, ErrorType errorType, string error)
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(errorType));
            Assert.That(result.Error, Is.EqualTo(error));
            _jwtTokenService.Verify(
                x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IReadOnlyCollection<Role>>()),
                Times.Never);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
