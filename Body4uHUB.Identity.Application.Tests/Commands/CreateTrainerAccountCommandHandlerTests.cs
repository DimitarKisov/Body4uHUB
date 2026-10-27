using Body4uHUB.Identity.Application.Commands.CreateTrainer;
using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Identity.Application.Tests.Fakes;
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
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;

        private CreateTrainerAccountCommandHandler _handler;
        private User _user;
        private Role _trainerRole;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _roleRepository = new Mock<IRoleRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _refreshTokenRepository = Sessions.EmptyRepository();

            _handler = new CreateTrainerAccountCommandHandler(
                _userRepository.Object,
                _roleRepository.Object,
                _refreshTokenRepository.Object,
                _unitOfWork.Object,
                TimeProvider.System);

            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", "test@mail.com", "0884787878", DateTime.UtcNow);
            _user.ClearDomainEvents();
            _trainerRole = Role.Create(TrainerRoleName);

            _userRepository
                .Setup(x => x.GetByIdAsync(_user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _roleRepository
                .Setup(x => x.FindByNameAsync(TrainerRoleName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_trainerRole);
        }

        [Test]
        public async Task Handle_ShouldAssignTrainerRoleAndRaiseEvent_WhenUserIsNotTrainer()
        {
            var result = await _handler.Handle(new CreateTrainerAccountCommand(_user.Id), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.HasRole(_trainerRole.Id), Is.True);
            Assert.That(_user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<TrainerRoleAssignedDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldEndEverySession_WhenTrainerRoleIsAssigned()
        {
            var sessions = new[] { Sessions.Active(_user.Id), Sessions.Active(_user.Id) };
            _refreshTokenRepository.SetupActive(_user.Id, sessions);
            var result = await _handler.Handle(new CreateTrainerAccountCommand(_user.Id), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(sessions.Select(x => x.RevokedAt), Has.All.Not.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnConflict_WhenUserIsAlreadyTrainer()
        {
            _user.AssignTrainerRole(_trainerRole);
            _user.ClearDomainEvents();

            var result = await _handler.Handle(new CreateTrainerAccountCommand(_user.Id), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.Error, Is.EqualTo(UserAlreadyTrainer));
            Assert.That(_user.DomainEvents, Is.Empty);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
