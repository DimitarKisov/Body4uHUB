using Body4uHUB.Identity.Application.Commands.Register;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
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
        private Mock<IEventBus> _eventBus;
        private Mock<IUnitOfWork> _unitOfWork;

        private RegisterCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _eventBus = new Mock<IEventBus>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _handler = new RegisterCommandHandler(
                _userRepository.Object,
                _passwordHasherService.Object,
                _eventBus.Object,
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
            _eventBus.Verify(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>()), Times.Never);
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
        public async Task Handle_ShouldCreateUserAndPublishEventBeforeSaving_WhenRegistrationIsValid()
        {
            var command = new RegisterCommand(ValidEmail, ValidPasswordHash, ValidFirstName, ValidLastName, ValidPhone);
            var calls = new List<string>();
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

            _eventBus
                .Setup(x => x.PublishAsync(It.IsAny<UserRegisteredEvent>()))
                .Callback(() => calls.Add("publish"))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => calls.Add("save"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Email, Is.EqualTo(ValidEmail));
            Assert.That(result.Value.IsEmailConfirmed, Is.False);
            Assert.That(addedUser, Is.Not.Null);
            Assert.That(result.Value.Id, Is.EqualTo(addedUser.Id));

            // The outbox only stores the message if it is published before SaveChanges.
            Assert.That(calls, Is.EqualTo(new[] { "publish", "save" }));
            _eventBus.Verify(
                x => x.PublishAsync(It.Is<UserRegisteredEvent>(e => e.UserId == addedUser.Id)),
                Times.Once);
        }
    }
}
