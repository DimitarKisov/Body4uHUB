using Body4uHUB.Content.Application.Commands.Forum.DeleteForumPost;
using Body4uHUB.Content.Application.Commands.Forum.EditForumPost;
using Body4uHUB.Content.Application.Tests.Fakes;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Domain.Exceptions;
using Moq;

namespace Body4uHUB.Content.Application.Tests.Commands;

[TestFixture]
public class EditDeleteForumPostCommandHandlerTests
{
    private const int TopicId = 5;
    private const int PostId = 9;
    private const string EditedContent = "Edited post content";

    private Mock<IForumRepository> _forumRepository;
    private Mock<IUnitOfWork> _unitOfWork;
    private Guid _postAuthorId;
    private ForumPost _post;

    [SetUp]
    public void Setup()
    {
        _forumRepository = new Mock<IForumRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _postAuthorId = Guid.NewGuid();

        // The repository loads the topic with only the requested post.
        var topic = ForumTopic.Create("Valid topic", Guid.NewGuid());
        _post = topic.AddPost("Valid post content", _postAuthorId).WithId(PostId);

        _forumRepository
            .Setup(x => x.GetByIdWithPostAsync(TopicId, PostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(topic);

        _forumRepository
            .Setup(x => x.GetByIdWithPostAsync(TopicId, It.Is<int>(y => y != PostId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ForumTopic.Create("Valid topic", Guid.NewGuid()));
    }

    private Task<Result> Edit(int topicId, int postId)
    {
        var handler = new EditForumPostCommandHandler(_forumRepository.Object, _unitOfWork.Object);
        return handler.Handle(new EditForumPostCommand(topicId, postId, EditedContent, AuthorizationContext.Create(_postAuthorId, false)), CancellationToken.None);
    }

    private Task<Result> Delete(int topicId, int postId)
    {
        var handler = new DeleteForumPostCommandHandler(_forumRepository.Object, _unitOfWork.Object);
        return handler.Handle(new DeleteForumPostCommand(topicId, postId, AuthorizationContext.Create(_postAuthorId, false)), CancellationToken.None);
    }

    [Test]
    public async Task Edit_ShouldUpdateContent()
    {
        var result = await Edit(TopicId, PostId);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(_post.Content, Is.EqualTo(EditedContent));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Edit_ShouldReturnNotFound_WhenTopicDoesNotExist()
    {
        var result = await Edit(TopicId + 1, PostId);

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
    }

    [Test]
    public void Edit_ShouldThrowDomainNotFoundException_WhenPostIsNotLoaded()
    {
        Assert.ThrowsAsync<DomainNotFoundException>(() => Edit(TopicId, PostId + 1));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Delete_ShouldMarkPostAsDeleted()
    {
        var result = await Delete(TopicId, PostId);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(_post.IsDeleted, Is.True);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Delete_ShouldReturnNotFound_WhenTopicDoesNotExist()
    {
        var result = await Delete(TopicId + 1, PostId);

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
    }

    [Test]
    public void Delete_ShouldThrowDomainNotFoundException_WhenPostIsNotLoaded()
    {
        Assert.ThrowsAsync<DomainNotFoundException>(() => Delete(TopicId, PostId + 1));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
