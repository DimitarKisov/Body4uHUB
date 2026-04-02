using Body4uHUB.Identity.Application.Commands.Register;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.Common;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class RegisterCommandHandlerTests
    {
        private const string ValidPasswordHash = "AQAAAAEAACcQAAAAEDummyHashValue==";
        private const string ValidFirstName = "Test";
        private const string ValidLastName = "User";
        private const string ValidEmail = "test@mail.com";
        private const string ValidPhone = "0884787878";
        private const string ValidJwtToken = "valid.jwt.token";

        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<IJwtTokenService> _jwtTokenService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IEmailService> _emailService;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<ILogger<RegisterCommandHandler>> _logger;

        private RegisterCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _jwtTokenService = new Mock<IJwtTokenService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _emailService = new Mock<IEmailService>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger<RegisterCommandHandler>>();

            _handler = new RegisterCommandHandler(
                _userRepository.Object,
                _passwordHasherService.Object,
                _jwtTokenService.Object,
                _unitOfWork.Object,
                _emailService.Object,
                _httpContextAccessor.Object,
                _logger.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(UserEmailExists));
        }

        [Test]
        public async Task Handle_ShouldReturnInternalServerError_WhenPasswordHashThrowsAnError()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(SomethingWentWrong));
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenEmailServiceThrows()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherService
                .Setup(x => x.HashPassword(command.Password))
                .Returns(ValidPasswordHash);

            _jwtTokenService
                .Setup(x => x.GenerateAccessToken(
                    It.IsAny<Guid>(),
                    command.Email,
                    It.IsAny<IReadOnlyCollection<Role>>()))
                .Returns(ValidJwtToken);

            _httpContextAccessor
                .SetupGet(x => x.HttpContext)
                .Returns((HttpContext)null);

            _emailService
                .Setup(x => x.SendEmailConfirmation(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("SMTP failure"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.AccessToken, Is.EqualTo(ValidJwtToken));
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenRegistrationIsValid()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherService
                .Setup(x => x.HashPassword(command.Password))
                .Returns(ValidPasswordHash);

            _jwtTokenService
                .Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), command.Email, It.IsAny<IReadOnlyCollection<Role>>()))
                .Returns(ValidJwtToken);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.AccessToken, Is.EqualTo(ValidJwtToken));
            Assert.That(result.Value.User, Is.Not.Null);
            Assert.That(result.Value.User.Email, Is.EqualTo(ValidEmail));

            _userRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _jwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<Guid>(), command.Email, It.IsAny<IReadOnlyCollection<Role>>()), Times.Once);
        }
    }
}
