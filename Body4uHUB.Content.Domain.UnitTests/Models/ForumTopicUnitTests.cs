using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared.Domain.Exceptions;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Domain.UnitTests.Models;

[TestFixture]
public class ForumTopicUnitTests
{
    private const string ValidTitle = "Valid topic";
    private const string PostContent = "First post content";
    private const string EditedContent = "Edited post content";
    private const int PostId = 1;

    private Guid _topicAuthorId;
    private Guid _postAuthorId;
    private ForumTopic _topic;
    private ForumPost _post;

    [SetUp]
    public void Setup()
    {
        _topicAuthorId = Guid.NewGuid();
        _postAuthorId = Guid.NewGuid();

        _topic = ForumTopic.Create(ValidTitle, _topicAuthorId);
        _post = _topic.AddPost(PostContent, _postAuthorId).WithId(PostId);
    }

    private static IEnumerable<TestCaseData> OperationsOnTopic()
    {
        var adminId = Guid.NewGuid();

        yield return new TestCaseData(new Action<ForumTopic>(t => t.AddPost(PostContent, adminId))).SetName("AddPost_ShouldThrow_WhenTopicIsDeleted");
        yield return new TestCaseData(new Action<ForumTopic>(t => t.EditPost(PostId, EditedContent, adminId, true))).SetName("EditPost_ShouldThrow_WhenTopicIsDeleted");
        yield return new TestCaseData(new Action<ForumTopic>(t => t.DeletePost(PostId, adminId, true))).SetName("DeletePost_ShouldThrow_WhenTopicIsDeleted");
        yield return new TestCaseData(new Action<ForumTopic>(t => t.Edit(ValidTitle, adminId, true))).SetName("Edit_ShouldThrow_WhenTopicIsDeleted");
        yield return new TestCaseData(new Action<ForumTopic>(t => t.Lock())).SetName("Lock_ShouldThrow_WhenTopicIsDeleted");
        yield return new TestCaseData(new Action<ForumTopic>(t => t.Unlock())).SetName("Unlock_ShouldThrow_WhenTopicIsDeleted");
    }

    [TestCaseSource(nameof(OperationsOnTopic))]
    public void Operation_ShouldThrowInvalidForumTopicException_WhenTopicIsDeleted(Action<ForumTopic> operation)
    {
        _topic.Delete(_topicAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidForumTopicException>(() => operation(_topic));

        Assert.That(ex.Error, Is.EqualTo(ForumTopicDeleted));
    }

    [Test]
    public void AddPost_ShouldReturnAddedPost()
    {
        var post = _topic.AddPost(PostContent, _postAuthorId);

        Assert.That(_topic.Posts, Does.Contain(post));
        Assert.That(post.AuthorId, Is.EqualTo(_postAuthorId));
    }

    [Test]
    public void AddPost_ShouldThrowInvalidForumTopicException_WhenTopicIsLocked()
    {
        _topic.Lock();

        var ex = Assert.Throws<InvalidForumTopicException>(() => _topic.AddPost(PostContent, _postAuthorId));

        Assert.That(ex.Error, Is.EqualTo(ForumTopicIsLocked));
    }

    [Test]
    public void Create_ShouldStartUnlockedAndNotDeleted()
    {
        Assert.That(_topic.IsLocked, Is.False);
        Assert.That(_topic.IsDeleted, Is.False);
        Assert.That(_topic.ViewCount, Is.Zero);
    }

    [Test]
    public void Unlock_ShouldUnlockTopic_WhenTopicIsLocked()
    {
        _topic.Lock();

        _topic.Unlock();

        Assert.That(_topic.IsLocked, Is.False);
    }

    [Test]
    public void EditPost_ShouldUpdateContent_WhenRequesterIsPostAuthor()
    {
        _topic.EditPost(PostId, EditedContent, _postAuthorId, isAdmin: false);

        Assert.That(_post.Content, Is.EqualTo(EditedContent));
    }

    [Test]
    public void EditPost_ShouldThrowDomainAuthorizationException_WhenRequesterIsNotPostAuthor()
    {
        var ex = Assert.Throws<DomainAuthorizationException>(() => _topic.EditPost(PostId, EditedContent, _topicAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ForumPostEditForbidden));
    }

    [Test]
    public void EditPost_ShouldThrowInvalidForumTopicException_WhenTopicIsLockedAndRequesterIsNotAdmin()
    {
        _topic.Lock();

        var ex = Assert.Throws<InvalidForumTopicException>(() => _topic.EditPost(PostId, EditedContent, _postAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ForumTopicIsLocked));
        Assert.That(_post.Content, Is.EqualTo(PostContent));
    }

    [Test]
    public void EditPost_ShouldUpdateContent_WhenTopicIsLockedAndRequesterIsAdmin()
    {
        _topic.Lock();

        _topic.EditPost(PostId, EditedContent, Guid.NewGuid(), isAdmin: true);

        Assert.That(_post.Content, Is.EqualTo(EditedContent));
    }

    [Test]
    public void EditPost_ShouldThrowDomainNotFoundException_WhenPostDoesNotExist()
    {
        var ex = Assert.Throws<DomainNotFoundException>(() => _topic.EditPost(PostId + 1, EditedContent, _postAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ForumPostNotFound));
    }

    [Test]
    public void EditPost_ShouldThrowInvalidForumPostException_WhenPostIsDeleted()
    {
        _topic.DeletePost(PostId, _postAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidForumPostException>(() => _topic.EditPost(PostId, EditedContent, _postAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ForumPostDeleted));
        Assert.That(_post.Content, Is.EqualTo(PostContent));
    }

    [Test]
    public void DeletePost_ShouldMarkPostAsDeleted_WhenRequesterIsPostAuthor()
    {
        _topic.DeletePost(PostId, _postAuthorId, isAdmin: false);

        Assert.That(_post.IsDeleted, Is.True);
    }

    [Test]
    public void DeletePost_ShouldThrowInvalidForumTopicException_WhenTopicIsLockedAndRequesterIsNotAdmin()
    {
        _topic.Lock();

        var ex = Assert.Throws<InvalidForumTopicException>(() => _topic.DeletePost(PostId, _postAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ForumTopicIsLocked));
        Assert.That(_post.IsDeleted, Is.False);
    }

    [Test]
    public void DeletePost_ShouldMarkPostAsDeleted_WhenTopicIsLockedAndRequesterIsAdmin()
    {
        _topic.Lock();

        _topic.DeletePost(PostId, Guid.NewGuid(), isAdmin: true);

        Assert.That(_post.IsDeleted, Is.True);
    }

    [Test]
    public void DeletePost_ShouldThrowInvalidForumPostException_WhenPostIsAlreadyDeleted()
    {
        _topic.DeletePost(PostId, _postAuthorId, isAdmin: false);

        var ex = Assert.Throws<InvalidForumPostException>(() => _topic.DeletePost(PostId, _postAuthorId, isAdmin: false));

        Assert.That(ex.Error, Is.EqualTo(ForumPostAlreadyDeleted));
    }
}
