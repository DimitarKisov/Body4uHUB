using Body4uHUB.Content.Application.Commands.Articles.Archive;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Tests.Commands;

[TestFixture]
public class ArchiveArticleCommandHandlerTests
{
    private const int ArticleId = 3;

    private Mock<IArticleRepository> _articleRepository;
    private Mock<IUnitOfWork> _unitOfWork;
    private Guid _authorId;
    private Article _article;

    [SetUp]
    public void Setup()
    {
        _articleRepository = new Mock<IArticleRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _authorId = Guid.NewGuid();

        _article = Article.Create("Valid article title", new string('a', ContentMinLength), _authorId);
        _article.Publish(_authorId, isAdmin: false, DateTime.UtcNow);

        _articleRepository
            .Setup(x => x.GetByIdAsync(ArticleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_article);
    }

    private Task<Result> Archive()
    {
        var handler = new ArchiveArticleCommandHandler(_articleRepository.Object, _unitOfWork.Object);
        return handler.Handle(new ArchiveArticleCommand(ArticleId, AuthorizationContext.Create(_authorId, false)), CancellationToken.None);
    }

    [Test]
    public async Task ShouldArchiveWithoutLoadingComments()
    {
        var result = await Archive();

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(_article.Status, Is.EqualTo(ArticleStatus.Archived));
        _articleRepository.Verify(x => x.GetWithCommentsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        _articleRepository
            .Setup(x => x.GetByIdAsync(ArticleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article)null);

        var result = await Archive();

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
