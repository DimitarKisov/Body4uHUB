using Body4uHUB.Content.Application.Commands.Forum.CreateForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.EditForumTopic;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Exceptions;
using Moq;

namespace Body4uHUB.Content.Application.Tests.Commands;

[TestFixture]
public class ForumTopicTitleCommandHandlerTests
{
    private const int TopicId = 4;
    private const string Title = "Valid topic";

    private Mock<IForumRepository> _forumRepository;
    private Mock<IUnitOfWork> _unitOfWork;
    private Guid _authorId;

    [SetUp]
    public void Setup()
    {
        _forumRepository = new Mock<IForumRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _authorId = Guid.NewGuid();

        _forumRepository
            .Setup(x => x.GetByIdAsync(TopicId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ForumTopic.Create(Title, _authorId));
    }

    private void SetupTitleExists(int? exceptTopicId, bool exists)
    {
        _forumRepository
            .Setup(x => x.ExistsByTitleAsync(Title, exceptTopicId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists);
    }

    private void SetupConcurrentDuplicateOnSave()
    {
        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UniqueConstraintViolationException(new Exception()));
    }

    private Task<Result<CreateForumTopicResponse>> Create()
    {
        var handler = new CreateForumTopicCommandHandler(_forumRepository.Object, _unitOfWork.Object);
        return handler.Handle(new CreateForumTopicCommand(Title, _authorId), CancellationToken.None);
    }

    private Task<Result> Edit()
    {
        var handler = new EditForumTopicCommandHandler(_forumRepository.Object, _unitOfWork.Object);
        return handler.Handle(new EditForumTopicCommand(TopicId, Title, AuthorizationContext.Create(_authorId, false)), CancellationToken.None);
    }

    [Test]
    public async Task Create_ShouldReturnConflict_WhenTitleExists()
    {
        SetupTitleExists(null, true);

        var result = await Create();

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
        _forumRepository.Verify(x => x.Add(It.IsAny<ForumTopic>()), Times.Never);
    }

    [Test]
    public async Task Create_ShouldReturnConflict_WhenConcurrentRequestSavedSameTitle()
    {
        SetupTitleExists(null, false);
        SetupConcurrentDuplicateOnSave();

        var result = await Create();

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
    }

    [Test]
    public async Task Create_ShouldSucceed_WhenTitleIsFree()
    {
        SetupTitleExists(null, false);

        var result = await Create();

        Assert.That(result.IsSuccess, Is.True);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Edit_ShouldIgnoreTheEditedTopic_WhenCheckingTitle()
    {
        SetupTitleExists(TopicId, false);

        var result = await Edit();

        Assert.That(result.IsSuccess, Is.True);
        _forumRepository.Verify(x => x.ExistsByTitleAsync(Title, TopicId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Edit_ShouldReturnConflict_WhenAnotherTopicHasTitle()
    {
        SetupTitleExists(TopicId, true);

        var result = await Edit();

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Edit_ShouldReturnConflict_WhenConcurrentRequestSavedSameTitle()
    {
        SetupTitleExists(TopicId, false);
        SetupConcurrentDuplicateOnSave();

        var result = await Edit();

        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
    }
}
