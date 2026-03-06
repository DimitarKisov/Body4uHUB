using Body4uHUB.Identity.Application.Commands.Register;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

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
        private const string ValidToken = "someRandomConfirmationToken";

        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<IJwtTokenService> _jwtTokenService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IEmailService> _emailService;
        private Mock<HttpContextAccessor> _httpContextAccessor;
        private Mock<ILogger<RegisterCommand.RegisterCommandHandler>> _logger;

        private RegisterCommand.RegisterCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _jwtTokenService = new Mock<IJwtTokenService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _emailService = new Mock<IEmailService>();
            _httpContextAccessor = new Mock<HttpContextAccessor>();
            _logger = new Mock<ILogger<RegisterCommand.RegisterCommandHandler>>();

            _handler = new RegisterCommand.RegisterCommandHandler(
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
            var command = new RegisterCommand
            {
                Email = ValidEmail,
                Password = ValidPasswordHash,
                FirstName = ValidFirstName,
                LastName = ValidLastName,
                PhoneNumber = ValidPhone
            };

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsFailure);
            Assert.That(result.Error, Is.EqualTo(UserEmailExists));
        }
    }
}
