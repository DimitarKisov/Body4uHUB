using Body4uHUB.Identity.Application.Queries.GetPublicUsers;
using Body4uHUB.Identity.Application.Repositories;
using Moq;

namespace Body4uHUB.Identity.Application.Tests.Queries
{
    [TestFixture]
    public class GetPublicUsersQueryHandlerTests
    {
        private Mock<IUserReadRepository> _userReadRepository;

        private GetPublicUsersQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userReadRepository = new Mock<IUserReadRepository>();
            _handler = new GetPublicUsersQueryHandler(_userReadRepository.Object);
        }

        [Test]
        public async Task Handle_ShouldQueryEachIdOnce_AndReturnTheFoundUsers()
        {
            var user = new PublicUserResponse(Guid.NewGuid(), "Test", "User");
            var unknownId = Guid.NewGuid();
            IReadOnlyCollection<Guid> queriedIds = null;

            _userReadRepository
                .Setup(x => x.GetPublicByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
                .Callback<IReadOnlyCollection<Guid>, CancellationToken>((ids, _) => queriedIds = ids)
                .ReturnsAsync([user]);

            var result = await _handler.Handle(new GetPublicUsersQuery([user.Id, unknownId, user.Id]), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { user }));
            Assert.That(queriedIds, Is.EquivalentTo(new[] { user.Id, unknownId }));
        }
    }
}
