using Body4uHUB.Identity.Application.Commands.DeleteTrainer;
using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class DeleteTrainerCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IRoleRepository> _roleRepository;
        private Mock<IEventBus> _eventBus;
        private Mock<IUnitOfWork> _unitOfWork;

        private DeleteTrainerCommandHandler _handler;
        private User _user;
        private Role _trainerRole;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _roleRepository = new Mock<IRoleRepository>();
            _eventBus = new Mock<IEventBus>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _handler = new DeleteTrainerCommandHandler(
                _userRepository.Object,
                _roleRepository.Object,
                _eventBus.Object,
                _unitOfWork.Object);

            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", "test@mail.com", "0884787878");
            _trainerRole = Role.Create(TrainerRoleName);

            _userRepository
                .Setup(x => x.GetByIdAsync(_user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _roleRepository
                .Setup(x => x.FindByNameAsync(TrainerRoleName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_trainerRole);
        }

        [Test]
        public async Task Handle_ShouldRevokeTrainerRoleAndPublishEvent_WhenUserIsTrainer()
        {
            _user.AssignTrainerRole(_trainerRole);

            var result = await _handler.Handle(new DeleteTrainerCommand(_user.Id), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.IsTrainer, Is.False);
            _eventBus.Verify(
                x => x.PublishAsync(It.Is<TrainerAccountDeletedEvent>(e => e.UserId == _user.Id), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrowInvalidUserException_WhenUserIsNotTrainer()
        {
            var ex = Assert.ThrowsAsync<InvalidUserException>(() => _handler.Handle(new DeleteTrainerCommand(_user.Id), CancellationToken.None));

            Assert.That(ex.Error, Is.EqualTo(UserNotInRole));
            _eventBus.Verify(x => x.PublishAsync(It.IsAny<TrainerAccountDeletedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
