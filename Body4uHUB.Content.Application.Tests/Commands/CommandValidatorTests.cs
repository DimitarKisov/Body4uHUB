using Body4uHUB.Content.Application.Commands.Articles.Archive;
using Body4uHUB.Content.Application.Commands.Articles.CreateComment;
using Body4uHUB.Content.Application.Commands.Articles.EditComment;
using Body4uHUB.Content.Application.Commands.Bookmarks.AddBookmark;
using Body4uHUB.Content.Application.Commands.Forum.CreateForumPost;
using Body4uHUB.Content.Application.Commands.Forum.CreateForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.DeleteForumPost;
using Body4uHUB.Content.Application.Commands.Forum.EditForumPost;
using Body4uHUB.Content.Application.Commands.Forum.LockForumTopic;
using Body4uHUB.Shared.Application;
using FluentValidation;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommonConstants;

namespace Body4uHUB.Content.Application.Tests.Commands;

[TestFixture]
public class CommandValidatorTests
{
    private static readonly AuthorizationContext AuthContext = AuthorizationContext.Create(Guid.NewGuid(), false);

    private static IEnumerable<TestCaseData> TooShortContent()
    {
        yield return new TestCaseData(new CreateCommentCommandValidator(), new CreateCommentCommand("a", 1, Guid.NewGuid(), null), "Content must be between 2 and 1000 characters long.")
            .SetName("CreateComment_ShouldFormatLengthMessage");
        yield return new TestCaseData(new EditCommentCommandValidator(), new EditCommentCommand(1, Guid.NewGuid(), "a", AuthContext), "Content must be between 2 and 1000 characters long.")
            .SetName("EditComment_ShouldFormatLengthMessage");
        yield return new TestCaseData(new CreateForumPostCommandValidator(), new CreateForumPostCommand("abc", 1, Guid.NewGuid()), "Content must be between 5 and 2000 characters long.")
            .SetName("CreateForumPost_ShouldFormatLengthMessage");
        yield return new TestCaseData(new EditForumPostCommandValidator(), new EditForumPostCommand(1, 1, "abc", AuthContext), "Content must be between 5 and 2000 characters long.")
            .SetName("EditForumPost_ShouldFormatLengthMessage");
        yield return new TestCaseData(new CreateForumTopicCommandValidator(), new CreateForumTopicCommand("abc", Guid.NewGuid()), "Title must be between 5 and 100 characters long.")
            .SetName("CreateForumTopic_ShouldFormatLengthMessage");
    }

    [TestCaseSource(nameof(TooShortContent))]
    public void Validate_ShouldReturnFormattedLengthMessage<T>(IValidator<T> validator, T command, string expectedMessage)
    {
        var result = validator.Validate(command);

        Assert.That(result.Errors.Select(e => e.ErrorMessage), Does.Contain(expectedMessage));
    }

    private static IEnumerable<TestCaseData> NegativeIds()
    {
        yield return new TestCaseData(new ArchiveArticleCommandValidator(), new ArchiveArticleCommand(-5, AuthContext), ArticleIdRequired)
            .SetName("ArchiveArticle_ShouldRejectNegativeId");
        yield return new TestCaseData(new AddBookmarkCommandValidator(), new AddBookmarkCommand(Guid.NewGuid(), -5), ArticleIdRequired)
            .SetName("AddBookmark_ShouldRejectNegativeArticleId");
        yield return new TestCaseData(new LockForumTopicCommandValidator(), new LockForumTopicCommand(-5), ForumTopicIdRequired)
            .SetName("LockForumTopic_ShouldRejectNegativeTopicId");
        yield return new TestCaseData(new DeleteForumPostCommandValidator(), new DeleteForumPostCommand(1, -5, AuthContext), ForumPostIdRequired)
            .SetName("DeleteForumPost_ShouldRejectNegativePostId");
    }

    [TestCaseSource(nameof(NegativeIds))]
    public void Validate_ShouldRejectNegativeId<T>(IValidator<T> validator, T command, string expectedMessage)
    {
        var result = validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Select(e => e.ErrorMessage), Does.Contain(expectedMessage));
    }
}
