using Body4uHUB.Identity.Application.Commands.AddUserRoles;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class AddUserRolesCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IRoleRepository> _roleRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private AddUserRolesCommandHandler _handler;
        private User _user;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _roleRepository = new Mock<IRoleRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _handler = new AddUserRolesCommandHandler(
                _userRepository.Object,
                _roleRepository.Object,
                _unitOfWork.Object);

            _user = User.Create("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", "test@mail.com", "0884787878", DateTime.UtcNow);

            _userRepository
                .Setup(x => x.GetByIdAsync(_user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
        }

        [Test]
        public async Task Handle_ShouldAddRoles_WhenRolesAreValid()
        {
            var role = Role.Create("Administrator");
            SetupRoles(role);

            var result = await _handler.Handle(new AddUserRolesCommand(_user.Id, [role.Id]), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_user.HasRole(role.Id), Is.True);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnBusinessRuleViolation_WhenRolesIncludeTrainer()
        {
            var otherRole = Role.Create("Administrator");
            var trainerRole = Role.Create(TrainerRoleName);
            SetupRoles(otherRole, trainerRole);

            var result = await _handler.Handle(
                new AddUserRolesCommand(_user.Id, [otherRole.Id, trainerRole.Id]),
                CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.BusinessRule));
            Assert.That(result.Error, Is.EqualTo(TrainerRoleRequiresTrainerAccount));
            Assert.That(_user.HasRole(otherRole.Id), Is.False);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private void SetupRoles(params Role[] roles)
        {
            _roleRepository
                .Setup(x => x.FindByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles.ToList());
        }
    }
}
