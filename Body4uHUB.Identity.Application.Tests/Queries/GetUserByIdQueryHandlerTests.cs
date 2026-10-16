using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Queries.GetUserById;
using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Shared.Application;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Queries
{
    [TestFixture]
    public class GetUserByIdQueryHandlerTests
    {
        private Mock<IUserReadRepository> _userReadRepository;

        private GetUserByIdQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userReadRepository = new Mock<IUserReadRepository>();
            _handler = new GetUserByIdQueryHandler(_userReadRepository.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            var user = new UserDto(Guid.NewGuid(), "test@mail.com", "Test", "User", null, DateTime.UtcNow, true, []);

            _userReadRepository
                .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(user));
        }

        [Test]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var result = await _handler.Handle(new GetUserByIdQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
            Assert.That(result.Error, Is.EqualTo(UserNotFound));
        }
    }
}
