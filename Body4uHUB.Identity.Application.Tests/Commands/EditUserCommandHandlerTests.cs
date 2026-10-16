using Body4uHUB.Identity.Application.Commands.EditUser;
using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class EditUserCommandHandlerTests
    {
        private const string Email = "test@mail.com";

        private Mock<IUserRepository> _userRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private User _user;

        private EditUserCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", Email, "0884787878", DateTime.UtcNow);

            _userRepository
                .Setup(x => x.GetByIdAsync(_user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _handler = new EditUserCommandHandler(_userRepository.Object, _unitOfWork.Object);
        }

        [Test]
        public async Task Handle_ShouldUpdateProfileAndKeepEmail_WhenUserExists()
        {
            var result = await _handler.Handle(new EditUserCommand(_user.Id, "Ivan", "Petrov", "+359884787878"), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.FirstName, Is.EqualTo("Ivan"));
            Assert.That(_user.LastName, Is.EqualTo("Petrov"));
            Assert.That(_user.ContactInfo.PhoneNumber, Is.EqualTo("+359884787878"));
            Assert.That(_user.ContactInfo.Email, Is.EqualTo(Email));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var result = await _handler.Handle(new EditUserCommand(Guid.NewGuid(), "Ivan", "Petrov", null), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
            Assert.That(result.Error, Is.EqualTo(UserNotFound));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Handle_ShouldThrowAndNotSave_WhenDomainRejectsTheName()
        {
            Assert.ThrowsAsync<InvalidUserException>(() =>
                _handler.Handle(new EditUserCommand(_user.Id, "I", "Petrov", null), CancellationToken.None));

            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
