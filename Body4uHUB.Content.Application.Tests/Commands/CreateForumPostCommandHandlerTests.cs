using Body4uHUB.Content.Application.Commands.Forum.CreateForumPost;
using Body4uHUB.Content.Application.Tests.Fakes;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

namespace Body4uHUB.Content.Application.Tests.Commands;

[TestFixture]
public class CreateForumPostCommandHandlerTests
{
    private const int TopicId = 5;
    private const int GeneratedPostId = 42;

    private Mock<IForumRepository> _forumRepository;
    private Mock<IUnitOfWork> _unitOfWork;
    private CreateForumPostCommandHandler _handler;
    private ForumTopic _topic;

    [SetUp]
    public void Setup()
    {
        _forumRepository = new Mock<IForumRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateForumPostCommandHandler(_forumRepository.Object, _unitOfWork.Object);

        _topic = ForumTopic.Create("Valid topic", Guid.NewGuid());

        _forumRepository
            .Setup(x => x.GetByIdAsync(TopicId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_topic);

        // Simulates the database assigning the identity value on save.
        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => _topic.Posts.Last().WithId(GeneratedPostId))
            .ReturnsAsync(1);
    }

    [Test]
    public async Task Handle_ShouldReturnPostIdAssignedOnSave()
    {
        var result = await _handler.Handle(new CreateForumPostCommand("Valid post content", TopicId, Guid.NewGuid()), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.EqualTo(GeneratedPostId));
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenTopicDoesNotExist()
    {
        var result = await _handler.Handle(new CreateForumPostCommand("Valid post content", TopicId + 1, Guid.NewGuid()), CancellationToken.None);

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ResourceNotFound));
    }
}
