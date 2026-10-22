using Body4uHUB.Content.Application.Commands.Articles.CreateComment;
using Body4uHUB.Content.Application.Commands.Articles.DeleteComment;
using Body4uHUB.Content.Application.Commands.Articles.EditComment;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Tests.Commands;

[TestFixture]
public class CommentCommandHandlerTests
{
    private const int ArticleId = 3;
    private const string CommentContent = "Nice article";
    private const string EditedCommentContent = "Great article";

    private Mock<IArticleRepository> _articleRepository;
    private Mock<IUnitOfWork> _unitOfWork;
    private Article _article;
    private Guid _commentAuthorId;
    private Guid _existingCommentId;

    [SetUp]
    public void Setup()
    {
        _articleRepository = new Mock<IArticleRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        var articleAuthorId = Guid.NewGuid();
        _commentAuthorId = Guid.NewGuid();

        _article = Article.Create("Valid article title", new string('a', ContentMinLength), articleAuthorId);
        _article.Publish(articleAuthorId, isAdmin: false, DateTime.UtcNow);
        _existingCommentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        _articleRepository
            .Setup(x => x.GetWithCommentsByIdAsync(ArticleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_article);
    }

    [Test]
    public async Task CreateComment_ShouldAddReply_WhenParentCommentExists()
    {
        var handler = new CreateCommentCommandHandler(_articleRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(
            new CreateCommentCommand(CommentContent, ArticleId, Guid.NewGuid(), _existingCommentId), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(_article.Comments.Single(x => x.Id == result.Value.CommentId).ParentCommentId, Is.EqualTo(_existingCommentId));
        _articleRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateComment_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        var handler = new CreateCommentCommandHandler(_articleRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(
            new CreateCommentCommand(CommentContent, ArticleId + 1, Guid.NewGuid(), null), CancellationToken.None);

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task EditComment_ShouldUpdateContent_WhenRequesterIsCommentAuthor()
    {
        var handler = new EditCommentCommandHandler(_articleRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(
            new EditCommentCommand(ArticleId, _existingCommentId, EditedCommentContent, AuthorizationContext.Create(_commentAuthorId, false)), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(_article.Comments.Single().Content, Is.EqualTo(EditedCommentContent));
        _articleRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task EditComment_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        var handler = new EditCommentCommandHandler(_articleRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(
            new EditCommentCommand(ArticleId + 1, _existingCommentId, EditedCommentContent, AuthorizationContext.Create(_commentAuthorId, false)), CancellationToken.None);

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteComment_ShouldMarkCommentAsDeleted_WhenRequesterIsCommentAuthor()
    {
        var handler = new DeleteCommentCommandHandler(_articleRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(
            new DeleteCommentCommand(_existingCommentId, ArticleId, AuthorizationContext.Create(_commentAuthorId, false)), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(_article.Comments.Single().IsDeleted, Is.True);
        _articleRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
