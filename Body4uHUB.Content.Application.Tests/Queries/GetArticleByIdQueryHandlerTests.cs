using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles.GetById;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Moq;

namespace Body4uHUB.Content.Application.Tests.Queries
{
    [TestFixture]
    public class GetArticleByIdQueryHandlerTests
    {
        private const int ArticleId = 7;
        private const int ViewCount = 10;

        private Mock<IArticleRepository> _articleRepository;
        private Mock<IArticleReadRepository> _articleReadRepository;
        private GetArticleByIdQueryHandler _handler;
        private Guid _authorId;

        [SetUp]
        public void Setup()
        {
            _articleRepository = new Mock<IArticleRepository>();
            _articleReadRepository = new Mock<IArticleReadRepository>();
            _handler = new GetArticleByIdQueryHandler(_articleRepository.Object, _articleReadRepository.Object);
            _authorId = Guid.NewGuid();

            _articleRepository
                .Setup(x => x.IncrementViewCountAsync(ArticleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        private void SetupArticle(ArticleStatus status, params CommentDto[] comments)
        {
            _articleReadRepository
                .Setup(x => x.GetWithCommentsByIdAsync(ArticleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetArticleByIdResponse(
                    ArticleId, "Title", "Content", _authorId, status.Name, null, ViewCount, DateTime.UtcNow, null, comments.ToList()));
        }

        private Task<Result<GetArticleByIdResponse>> Send(AuthorizationContext authContext)
        {
            return _handler.Handle(new GetArticleByIdQuery(ArticleId, authContext), CancellationToken.None);
        }

        [Test]
        public async Task Handle_ShouldReturnNotFound_WhenArticleDoesNotExist()
        {
            var result = await Send(null);

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
        }

        [Test]
        public async Task Handle_ShouldReturnArticleAndIncrementViewCount_WhenArticleIsPublished()
        {
            SetupArticle(ArticleStatus.Published);

            var result = await Send(null);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.ViewCount, Is.EqualTo(ViewCount + 1));
            _articleRepository.Verify(x => x.IncrementViewCountAsync(ArticleId, It.IsAny<CancellationToken>()), Times.Once);
        }

        private static IEnumerable<TestCaseData> UnpublishedStatuses()
        {
            yield return new TestCaseData(ArticleStatus.Draft);
            yield return new TestCaseData(ArticleStatus.Archived);
        }

        [TestCaseSource(nameof(UnpublishedStatuses))]
        public async Task Handle_ShouldReturnNotFound_WhenArticleIsUnpublishedAndRequesterIsAnonymous(ArticleStatus status)
        {
            SetupArticle(status);

            var result = await Send(null);

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
        }

        [TestCaseSource(nameof(UnpublishedStatuses))]
        public async Task Handle_ShouldReturnNotFound_WhenArticleIsUnpublishedAndRequesterIsAnotherUser(ArticleStatus status)
        {
            SetupArticle(status);

            var result = await Send(AuthorizationContext.Create(Guid.NewGuid(), false));

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
        }

        [TestCaseSource(nameof(UnpublishedStatuses))]
        public async Task Handle_ShouldReturnArticleWithoutCountingView_WhenArticleIsUnpublishedAndRequesterIsAuthor(ArticleStatus status)
        {
            SetupArticle(status);

            var result = await Send(AuthorizationContext.Create(_authorId, false));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.ViewCount, Is.EqualTo(ViewCount));
            _articleRepository.Verify(x => x.IncrementViewCountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestCaseSource(nameof(UnpublishedStatuses))]
        public async Task Handle_ShouldReturnArticle_WhenArticleIsUnpublishedAndRequesterIsAdmin(ArticleStatus status)
        {
            SetupArticle(status);

            var result = await Send(AuthorizationContext.Create(Guid.NewGuid(), true));

            Assert.That(result.IsSuccess, Is.True);
        }

        // A (deleted) <- B (deleted) <- C;  D (deleted);  E (deleted) <- F (deleted)
        [Test]
        public async Task Handle_ShouldKeepDeletedCommentsOnlyWhenTheyHaveVisibleReplies()
        {
            var a = Comment(deleted: true);
            var b = Comment(deleted: true, parent: a);
            var c = Comment(deleted: false, parent: b);
            var d = Comment(deleted: true);
            var e = Comment(deleted: true);
            var f = Comment(deleted: true, parent: e);
            SetupArticle(ArticleStatus.Published, a, b, c, d, e, f);

            var result = await Send(null);

            var comments = result.Value.Comments;
            Assert.That(comments.Select(x => x.Id), Is.EqualTo(new[] { a.Id, b.Id, c.Id }));
            Assert.That(comments.Single(x => x.Id == a.Id).Content, Is.Null);
            Assert.That(comments.Single(x => x.Id == b.Id).Content, Is.Null);
            Assert.That(comments.Single(x => x.Id == c.Id).Content, Is.Not.Null);
        }

        private static CommentDto Comment(bool deleted, CommentDto parent = null)
        {
            return new CommentDto
            {
                Id = Guid.NewGuid(),
                Content = "Comment content",
                AuthorId = Guid.NewGuid(),
                ParentCommentId = parent?.Id,
                IsDeleted = deleted,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
