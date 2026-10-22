using Body4uHUB.Content.Application.Commands.Bookmarks.AddBookmark;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Exceptions;
using Moq;

namespace Body4uHUB.Content.Application.Tests.Commands
{
    [TestFixture]
    public class AddBookmarkCommandHandlerTests
    {
        private const int ArticleId = 7;

        private Mock<IBookmarkRepository> _bookmarkRepository;
        private Mock<IArticleRepository> _articleRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _bookmarkRepository = new Mock<IBookmarkRepository>();
            _articleRepository = new Mock<IArticleRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _userId = Guid.NewGuid();

            _articleRepository
                .Setup(x => x.ExistsPublishedAsync(ArticleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        private Task<Result<AddBookmarkResponse>> Add()
        {
            var handler = new AddBookmarkCommandHandler(_bookmarkRepository.Object, _articleRepository.Object, _unitOfWork.Object);
            return handler.Handle(new AddBookmarkCommand(_userId, ArticleId), CancellationToken.None);
        }

        [Test]
        public async Task ShouldAddBookmark_WhenArticleIsPublished()
        {
            var result = await Add();

            Assert.That(result.IsSuccess, Is.True);
            _bookmarkRepository.Verify(x => x.Add(It.Is<Bookmark>(y => y.UserId == _userId && y.ArticleId == ArticleId)), Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ShouldReturnNotFound_WhenArticleIsNotPublished()
        {
            _articleRepository
                .Setup(x => x.ExistsPublishedAsync(ArticleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await Add();

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
            _bookmarkRepository.Verify(x => x.Add(It.IsAny<Bookmark>()), Times.Never);
        }

        [Test]
        public async Task ShouldReturnConflict_WhenBookmarkExists()
        {
            _bookmarkRepository
                .Setup(x => x.ExistsAsync(_userId, ArticleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await Add();

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            _bookmarkRepository.Verify(x => x.Add(It.IsAny<Bookmark>()), Times.Never);
        }

        [Test]
        public async Task ShouldReturnConflict_WhenConcurrentRequestSavedSameBookmark()
        {
            _unitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UniqueConstraintViolationException(new Exception()));

            var result = await Add();

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
        }
    }
}
