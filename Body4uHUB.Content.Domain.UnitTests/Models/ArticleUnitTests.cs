using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared.Domain.Exceptions;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Domain.UnitTests.Models;

[TestFixture]
public class ArticleUnitTests
{
    private const string ValidTitle = "Valid article title";
    private const string CommentContent = "Nice article";
    private const string EditedCommentContent = "Great article";

    private static readonly string ValidContent = new('a', ContentMinLength);
    private static readonly DateTime PublishedAtUtc = new(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);

    private Guid _articleAuthorId;
    private Guid _commentAuthorId;
    private Article _article;

    [SetUp]
    public void Setup()
    {
        _articleAuthorId = Guid.NewGuid();
        _commentAuthorId = Guid.NewGuid();

        _article = Article.Create(ValidTitle, ValidContent, _articleAuthorId);
        _article.Publish(_articleAuthorId, isAdmin: false, PublishedAtUtc);
    }

    [Test]
    public void AddComment_ShouldAddComment_WhenArticleIsPublished()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        Assert.That(_article.Comments.Single().Id, Is.EqualTo(commentId));
    }

    [Test]
    public void AddComment_ShouldAddReply_WhenParentCommentExists()
    {
        var parentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        var replyId = _article.AddComment(CommentContent, _articleAuthorId, parentId);

        Assert.That(_article.Comments.Single(x => x.Id == replyId).ParentCommentId, Is.EqualTo(parentId));
    }

    [Test]
    public void AddComment_ShouldThrowDomainNotFoundException_WhenParentCommentDoesNotExist()
    {
        var ex = Assert.Throws<DomainNotFoundException>(() => _article.AddComment(CommentContent, _commentAuthorId, Guid.NewGuid()));

        Assert.That(ex.Error, Is.EqualTo(CommentParentNotFound));
    }

    [Test]
    public void AddComment_ShouldThrowInvalidArticleException_WhenArticleIsNotPublished()
    {
        var draft = Article.Create(ValidTitle, ValidContent, _articleAuthorId);

        var ex = Assert.Throws<InvalidArticleException>(() => draft.AddComment(CommentContent, _commentAuthorId, null));

        Assert.That(ex.Error, Is.EqualTo(ArticleNotPublished));
    }

    [Test]
    public void AddComment_ShouldThrowInvalidArticleException_WhenArticleIsDeleted()
    {
        _article.Delete(_articleAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidArticleException>(() => _article.AddComment(CommentContent, _commentAuthorId, null));

        Assert.That(ex.Error, Is.EqualTo(ArticleAlreadyDeleted));
    }

    [Test]
    public void EditComment_ShouldUpdateContent_WhenRequesterIsCommentAuthor()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        _article.EditComment(commentId, EditedCommentContent, _commentAuthorId, isAdmin: false);

        Assert.That(_article.Comments.Single().Content, Is.EqualTo(EditedCommentContent));
    }

    [Test]
    public void EditComment_ShouldUpdateContent_WhenRequesterIsAdmin()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        _article.EditComment(commentId, EditedCommentContent, Guid.NewGuid(), isAdmin: true);

        Assert.That(_article.Comments.Single().Content, Is.EqualTo(EditedCommentContent));
    }

    [Test]
    public void EditComment_ShouldThrowDomainAuthorizationException_WhenRequesterIsOnlyArticleAuthor()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        var ex = Assert.Throws<DomainAuthorizationException>(() => _article.EditComment(commentId, EditedCommentContent, _articleAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(CommentEditForbidden));
        Assert.That(_article.Comments.Single().Content, Is.EqualTo(CommentContent));
    }

    [Test]
    public void EditComment_ShouldThrowDomainNotFoundException_WhenCommentDoesNotExist()
    {
        var ex = Assert.Throws<DomainNotFoundException>(() => _article.EditComment(Guid.NewGuid(), EditedCommentContent, _commentAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(CommentNotFound));
    }

    [Test]
    public void EditComment_ShouldThrowInvalidCommentException_WhenCommentIsDeleted()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);
        _article.DeleteComment(commentId, _commentAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidCommentException>(() => _article.EditComment(commentId, EditedCommentContent, _commentAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(CommentAlreadyDeleted));
    }

    [Test]
    public void EditComment_ShouldThrowInvalidArticleException_WhenArticleIsArchived()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);
        _article.Archive(_articleAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidArticleException>(() => _article.EditComment(commentId, EditedCommentContent, _commentAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ArticleNotPublished));
        Assert.That(_article.Comments.Single().Content, Is.EqualTo(CommentContent));
    }

    [Test]
    public void DeleteComment_ShouldMarkCommentAsDeleted_WhenRequesterIsCommentAuthor()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        _article.DeleteComment(commentId, _commentAuthorId, isAdmin: false);

        Assert.That(_article.Comments.Single().IsDeleted, Is.True);
    }

    [Test]
    public void DeleteComment_ShouldMarkCommentAsDeleted_WhenRequesterIsAdmin()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        _article.DeleteComment(commentId, Guid.NewGuid(), isAdmin: true);

        Assert.That(_article.Comments.Single().IsDeleted, Is.True);
    }

    [Test]
    public void DeleteComment_ShouldThrowDomainAuthorizationException_WhenRequesterIsOnlyArticleAuthor()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);

        var ex = Assert.Throws<DomainAuthorizationException>(() => _article.DeleteComment(commentId, _articleAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(CommentDeleteForbidden));
        Assert.That(_article.Comments.Single().IsDeleted, Is.False);
    }

    [Test]
    public void DeleteComment_ShouldThrowDomainNotFoundException_WhenCommentDoesNotExist()
    {
        var ex = Assert.Throws<DomainNotFoundException>(() => _article.DeleteComment(Guid.NewGuid(), _commentAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(CommentNotFound));
    }

    [Test]
    public void DeleteComment_ShouldThrowInvalidCommentException_WhenCommentIsAlreadyDeleted()
    {
        var commentId = _article.AddComment(CommentContent, _commentAuthorId, null);
        _article.DeleteComment(commentId, _commentAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidCommentException>(() => _article.DeleteComment(commentId, _commentAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(CommentAlreadyDeleted));
    }

    [Test]
    public void Archive_ShouldArchiveArticle_WhenArticleIsPublished()
    {
        _article.Archive(_articleAuthorId, isAdmin: false);

        Assert.That(_article.Status, Is.EqualTo(ArticleStatus.Archived));
    }

    [Test]
    public void Archive_ShouldKeepPublishedAt()
    {
        _article.Archive(_articleAuthorId, isAdmin: false);

        Assert.That(_article.PublishedAt, Is.EqualTo(PublishedAtUtc));
    }

    [Test]
    public void Publish_ShouldSetPublishedAtToGivenTime()
    {
        Assert.That(_article.PublishedAt, Is.EqualTo(PublishedAtUtc));
    }

    [Test]
    public void Publish_ShouldKeepFirstPublishedAt_WhenArchivedArticleIsPublishedAgain()
    {
        _article.Archive(_articleAuthorId, isAdmin: false);

        _article.Publish(_articleAuthorId, isAdmin: false, PublishedAtUtc.AddDays(30));

        Assert.That(_article.Status, Is.EqualTo(ArticleStatus.Published));
        Assert.That(_article.PublishedAt, Is.EqualTo(PublishedAtUtc));
    }

    [Test]
    public void Publish_ShouldThrowArgumentException_WhenTimeIsNotUtc()
    {
        var draft = Article.Create(ValidTitle, ValidContent, _articleAuthorId);

        Assert.Throws<ArgumentException>(() => draft.Publish(_articleAuthorId, isAdmin: false, DateTime.Now));
        Assert.That(draft.Status, Is.EqualTo(ArticleStatus.Draft));
    }

    [Test]
    public void Archive_ShouldThrowInvalidArticleException_WhenArticleIsDeleted()
    {
        _article.Delete(_articleAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidArticleException>(() => _article.Archive(_articleAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ArticleAlreadyDeleted));
    }
}
