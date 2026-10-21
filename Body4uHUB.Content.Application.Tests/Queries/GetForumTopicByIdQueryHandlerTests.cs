using Body4uHUB.Content.Application.Abstractions;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Forum.GetById;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Repositories;
using Moq;

namespace Body4uHUB.Content.Application.Tests.Queries
{
    [TestFixture]
    public class GetForumTopicByIdQueryHandlerTests
    {
        private const int TopicId = 9;
        private const int ViewCount = 3;
        private const string VisitorIp = "203.0.113.5";

        private Mock<IForumRepository> _forumRepository;
        private Mock<IForumReadRepository> _forumReadRepository;
        private Mock<IViewTracker> _viewTracker;
        private GetForumTopicByIdQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _forumRepository = new Mock<IForumRepository>();
            _forumReadRepository = new Mock<IForumReadRepository>();
            _viewTracker = new Mock<IViewTracker>();
            _handler = new GetForumTopicByIdQueryHandler(_forumRepository.Object, _forumReadRepository.Object, _viewTracker.Object);

            _forumReadRepository
                .Setup(x => x.GetByIdAsync(TopicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetForumTopicByIdResponse(
                    TopicId, "Valid topic", Guid.NewGuid(), false, ViewCount, 0, DateTime.UtcNow, null, new List<ForumPostDto>()));

            _forumRepository
                .Setup(x => x.IncrementViewCountAsync(TopicId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        private void SetupViewAlreadyCountedToday(bool alreadyCounted)
        {
            _viewTracker
                .Setup(x => x.TryRegisterViewAsync(ViewTarget.ForumTopic, TopicId, VisitorIp, It.IsAny<CancellationToken>()))
                .ReturnsAsync(!alreadyCounted);
        }

        [Test]
        public async Task Handle_ShouldIncrementViewCount_WhenFirstViewOfTheDay()
        {
            SetupViewAlreadyCountedToday(false);

            var result = await _handler.Handle(new GetForumTopicByIdQuery(TopicId, VisitorIp), CancellationToken.None);

            Assert.That(result.Value.ViewCount, Is.EqualTo(ViewCount + 1));
            _forumRepository.Verify(x => x.IncrementViewCountAsync(TopicId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldNotIncrementViewCount_WhenVisitorWasAlreadyCountedToday()
        {
            SetupViewAlreadyCountedToday(true);

            var result = await _handler.Handle(new GetForumTopicByIdQuery(TopicId, VisitorIp), CancellationToken.None);

            Assert.That(result.Value.ViewCount, Is.EqualTo(ViewCount));
            _forumRepository.Verify(x => x.IncrementViewCountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
