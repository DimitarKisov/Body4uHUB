using Body4uHUB.Identity.Application.Commands.Register;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Exceptions;
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

        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<IUnitOfWork> _unitOfWork;

        private RegisterCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _handler = new RegisterCommandHandler(
                _userRepository.Object,
                _passwordHasherService.Object,
                _unitOfWork.Object);
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
            _userRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnConflict_WhenConcurrentRegistrationViolatesUniqueEmail()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherService
                .Setup(x => x.HashPassword(command.Password))
                .Returns(ValidPasswordHash);

            _unitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UniqueConstraintViolationException(new Exception()));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.Error, Is.EqualTo(UserEmailExists));
        }

        [Test]
        public void Handle_ShouldPropagateException_WhenPasswordHashThrows()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherService
                .Setup(x => x.HashPassword(It.IsAny<string>()))
                .Throws(new InvalidOperationException());

            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            _userRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldCreateUserAndRaiseEvent_WhenRegistrationIsValid()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);
            User addedUser = null;

            _userRepository
                .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherService
                .Setup(x => x.HashPassword(command.Password))
                .Returns(ValidPasswordHash);

            _userRepository
                .Setup(x => x.Add(It.IsAny<User>()))
                .Callback<User>(user => addedUser = user);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Email, Is.EqualTo(ValidEmail));
            Assert.That(result.Value.IsEmailConfirmed, Is.False);
            Assert.That(addedUser, Is.Not.Null);
            Assert.That(result.Value.Id, Is.EqualTo(addedUser.Id));
            Assert.That(addedUser.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(addedUser.DomainEvents[0], Is.TypeOf<UserRegisteredDomainEvent>().And.Property("UserId").EqualTo(addedUser.Id));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
