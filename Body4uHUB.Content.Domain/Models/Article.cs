using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Domain.Models;

public class Article : AggregateRoot<int>
{
    private readonly List<Comment> _comments = [];

    public string Title { get; private set; }
    public string Content { get; private set; }
    public ArticleCategory Category { get; private set; }
    public int ReadingMinutes { get; private set; }
    public ArticleStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public int ViewCount { get; private set; }
    public Guid AuthorId { get; private set; }
    public bool IsDeleted { get; private set; }
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private Article() { }

    private Article(string title, string content, ArticleCategory category, Guid authorId)
    {
        Title = title;
        Content = content;
        ReadingMinutes = CalculateReadingMinutes(content);
        Category = category;
        Status = ArticleStatus.Draft;
        AuthorId = authorId;
    }

    public static Article Create(string title, string content, ArticleCategory category, Guid authorId)
    {
        Validate(title, content, category, authorId);

        return new Article(title, content, category, authorId);
    }

    public void Edit(string title, string content, ArticleCategory category, Guid requesterId, bool isAdmin)
    {
        EnsureCanBeModifiedBy(requesterId, isAdmin);

        if (IsDeleted)
        {
            throw new InvalidArticleException(ArticleAlreadyDeleted);
        }

        UpdateTitle(title);
        UpdateContent(content);
        UpdateCategory(category);
    }

    public void Delete(Guid requesterId, bool isAdmin)
    {
        EnsureCanBeModifiedBy(requesterId, isAdmin);

        if (IsDeleted)
        {
            throw new InvalidArticleException(ArticleAlreadyDeleted);
        }

        MarkAsDeleted();
    }

    private void UpdateTitle(string title)
    {
        ValidateTitle(title);
        Title = title;
    }

    private void UpdateContent(string content)
    {
        ValidateContent(content);
        Content = content;
        ReadingMinutes = CalculateReadingMinutes(content);
    }

    private void UpdateCategory(ArticleCategory category)
    {
        ValidateCategory(category);
        Category = category;
    }

    // Stored rather than computed on read, so article lists do not have to load the content.
    private static int CalculateReadingMinutes(string content)
    {
        var wordCount = content.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;

        return Math.Max(1, (int)Math.Ceiling(wordCount / (double)WordsPerMinute));
    }

    public void Publish(Guid requesterId, bool isAdmin, DateTime utcNow)
    {
        EnsureUtc(utcNow);
        EnsureCanBeModifiedBy(requesterId, isAdmin);

        if (Status == ArticleStatus.Published)
        {
            throw new InvalidArticleException(ArticleAlreadyPublished);
        }
        else if (IsDeleted)
        {
            throw new InvalidArticleException(ArticleAlreadyDeleted);
        }

        Status = ArticleStatus.Published;

        // An archived article published again keeps the date of its first publication.
        PublishedAt ??= utcNow;
    }

    public void Archive(Guid requesterId, bool isAdmin)
    {
        EnsureCanBeModifiedBy(requesterId, isAdmin);

        if (IsDeleted)
        {
            throw new InvalidArticleException(ArticleAlreadyDeleted);
        }

        if (Status != ArticleStatus.Published)
        {
            throw new InvalidArticleException(ArticleNotPublished);
        }

        Status = ArticleStatus.Archived;
    }

    private void EnsureCanBeModifiedBy(Guid requesterId, bool isAdmin)
    {
        if (!isAdmin && AuthorId != requesterId)
        {
            throw new DomainAuthorizationException(ArticleModifyForbidden);
        }
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public Guid AddComment(string content, Guid authorId, Guid? parentCommentId)
    {
        if (IsDeleted)
        {
            throw new InvalidArticleException(ArticleAlreadyDeleted);
        }

        if (Status != ArticleStatus.Published)
        {
            throw new InvalidArticleException(ArticleNotPublished);
        }

        if (parentCommentId.HasValue && !_comments.Any(x => x.Id == parentCommentId.Value))
        {
            throw new DomainNotFoundException(CommentParentNotFound);
        }

        var comment = Comment.Create(content, authorId, parentCommentId);

        _comments.Add(comment);

        return comment.Id;
    }

    public void EditComment(Guid commentId, string content, Guid requesterId, bool isAdmin)
    {
        if (IsDeleted)
        {
            throw new InvalidArticleException(ArticleAlreadyDeleted);
        }

        if (Status != ArticleStatus.Published)
        {
            throw new InvalidArticleException(ArticleNotPublished);
        }

        var comment = _comments.FirstOrDefault(x => x.Id == commentId);

        if (comment is null)
        {
            throw new DomainNotFoundException(CommentNotFound);
        }

        if (!isAdmin && comment.AuthorId != requesterId)
        {
            throw new DomainAuthorizationException(CommentEditForbidden);
        }

        comment.UpdateContent(content);
    }

    // A draft or archived article is seen only by its author and admins, and its comments are read-only;
    // only an admin may still remove one (moderation). A deleted article is seen by no one.
    public void DeleteComment(Guid commentId, Guid requesterId, bool isAdmin)
    {
        if (IsDeleted)
        {
            throw new InvalidArticleException(ArticleAlreadyDeleted);
        }

        if (Status != ArticleStatus.Published && !isAdmin)
        {
            throw new InvalidArticleException(ArticleNotPublished);
        }

        var comment = _comments.FirstOrDefault(x => x.Id == commentId);

        if (comment is null)
        {
            throw new DomainNotFoundException(CommentNotFound);
        }

        if (!isAdmin && comment.AuthorId != requesterId)
        {
            throw new DomainAuthorizationException(CommentDeleteForbidden);
        }

        if (comment.IsDeleted)
        {
            throw new InvalidCommentException(CommentAlreadyDeleted);
        }

        comment.MarkAsDeleted();
    }

    private void MarkAsDeleted()
    {
        IsDeleted = true;
    }

    private static void Validate(string title, string content, ArticleCategory category, Guid authorId)
    {
        ValidateTitle(title);
        ValidateContent(content);
        ValidateCategory(category);
        ValidateAuthorId(authorId);
    }

    private static void ValidateTitle(string title)
    {
        Guard.AgainstEmptyString<InvalidArticleException>(title, nameof(Title));
        Guard.ForStringLength<InvalidArticleException>(title, TitleMinLength, TitleMaxLength, nameof(Title));
    }

    private static void ValidateContent(string content)
    {
        Guard.AgainstEmptyString<InvalidArticleException>(content, nameof(Content));
        Guard.ForStringLength<InvalidArticleException>(content, ContentMinLength, ContentMaxLength, nameof(Content));
    }

    private static void ValidateCategory(ArticleCategory category)
    {
        Guard.AgainstDefault<InvalidArticleException, ArticleCategory>(category, nameof(Category));
    }

    private static void ValidateAuthorId(Guid authorId)
    {
        Guard.AgainstEmptyGuid<InvalidArticleException>(authorId, nameof(AuthorId));
    }

    // The caller supplies "now", so a local time here is a programming error rather than invalid user input.
    private static void EnsureUtc(DateTime utcNow)
    {
        if (utcNow.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("The time must be in UTC.", nameof(utcNow));
        }
    }
}
