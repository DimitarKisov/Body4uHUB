using Body4uHUB.Identity.Application.Commands.CreateTrainer;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class CreateTrainerAccountCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IRoleRepository> _roleRepository;
        private Mock<IEventBus> _eventBus;
        private Mock<IUnitOfWork> _unitOfWork;

        private CreateTrainerAccountCommandHandler _handler;
        private User _user;
        private Role _trainerRole;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _roleRepository = new Mock<IRoleRepository>();
            _eventBus = new Mock<IEventBus>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _handler = new CreateTrainerAccountCommandHandler(
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
        public async Task Handle_ShouldAssignTrainerRoleAndPublishEvent_WhenUserIsNotTrainer()
        {
            var result = await _handler.Handle(new CreateTrainerAccountCommand(_user.Id), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.HasRole(_trainerRole.Id), Is.True);
            _eventBus.Verify(
                x => x.PublishAsync(It.Is<TrainerAccountCreatedEvent>(e => e.UserId == _user.Id), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnConflict_WhenUserIsAlreadyTrainer()
        {
            _user.AssignTrainerRole(_trainerRole);

            var result = await _handler.Handle(new CreateTrainerAccountCommand(_user.Id), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.Error, Is.EqualTo(UserAlreadyTrainer));
            _eventBus.Verify(x => x.PublishAsync(It.IsAny<TrainerAccountCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
