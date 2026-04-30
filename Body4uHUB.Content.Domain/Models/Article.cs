using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Domain.Models
{
    public class Article : AggregateRoot<int>
    {
        private readonly List<Comment> _comments = [];

        public string Title { get; private set; }
        public string Content { get; private set; }
        public ArticleStatus Status { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public int ViewCount { get; private set; }
        public Guid AuthorId { get; private set; }
        public bool IsDeleted { get; private set; }
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private Article()
            : base()
        { 
        }

        private Article(string title, string content, Guid authorId)
            : base()
        {
            Title = title;
            Content = content;
            Status = ArticleStatus.Draft;
            ViewCount = 0;
            AuthorId = authorId;
        }

        public static Article Create(string title, string content, Guid authorId)
        {
            Validate(title, content, authorId);

            return new Article(title, content, authorId);
        }

        public void Edit(string title, string content, Guid requesterId, bool isAdmin)
        {
            EnsureCanBeModifiedBy(requesterId, isAdmin);

            if (IsDeleted)
            {
                throw new InvalidArticleException(ArticleAlreadyDeleted);
            }

            UpdateTitle(title);
            UpdateContent(content);
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
        }

        public void Publish(Guid requesterId, bool isAdmin)
        {
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
            PublishedAt = DateTime.UtcNow;
        }

        public void Archive(Guid requesterId, bool isAdmin)
        {
            EnsureCanBeModifiedBy(requesterId, isAdmin);

            if (Status != ArticleStatus.Published)
            {
                throw new InvalidArticleException(ArticleNotPublished);
            }

            Status = ArticleStatus.Archived;
            PublishedAt = null;
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

        public Comment GetComment(Guid commentId)
        {
            var comment = _comments.FirstOrDefault(x => x.Id == commentId);
            if (comment is null)
            {
                throw new DomainNotFoundException(CommentNotFound);
            }

            return comment;
        }

        public Guid AddComment(string content, Guid authorId, Guid? parentCommentId)
        {
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

        public void DeleteComment(Guid commentId, Guid requesterId, bool isAdmin)
        {
            var comment = _comments.FirstOrDefault(x => x.Id == commentId);

            if (comment is null)
            {
                throw new DomainNotFoundException(CommentNotFound);
            }

            EnsureCanBeModifiedBy(requesterId, isAdmin);

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

        private static void Validate(string title, string content, Guid authorId)
        {
            ValidateTitle(title);
            ValidateContent(content);
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

        private static void ValidateAuthorId(Guid authorId)
        {
            Guard.AgainstEmptyGuid<InvalidArticleException>(authorId, nameof(AuthorId));
        }
    }
}
