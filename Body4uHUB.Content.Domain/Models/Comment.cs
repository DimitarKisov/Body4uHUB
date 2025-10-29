using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.Comment;

namespace Body4uHUB.Content.Domain.Models
{
    public class Comment : Entity<CommentId>, IAggregateRoot
    {
        public string Content { get; private set; }
        public Guid AuthorId { get; private set; }
        public ArticleId ArticleId { get; private set; }
        public CommentId ParentCommentId { get; private set; }
        public bool IsDeleted { get; private set; }

        private Comment()
            : base(default!)
        {
        }

        private Comment(string content, Guid authorId, ArticleId articleId, CommentId parentCommentId = null)
            : base(default!)
        {
            Content = content;
            AuthorId = authorId;
            ArticleId = articleId;
            ParentCommentId = parentCommentId;
            IsDeleted = false;
        }

        public static Comment Create(string content, Guid authorId, ArticleId articleId, CommentId parentCommentId = null)
        {
            Validate(content, authorId, articleId);
            return new Comment(content, authorId, articleId, parentCommentId);
        }

        public void UpdateContent(string content)
        {
            if (IsDeleted)
            {
                throw new InvalidCommentException(CommentAlreadyDeleted);
            }

            ValidateContent(content);
            Content = content;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        private static void Validate(string content, Guid authorId, ArticleId articleId)
        {
            ValidateContent(content);
            ValidateAuthorId(authorId);
            ValidateArticleId(articleId);
        }

        private static void ValidateContent(string content)
        {
            Guard.AgainstEmptyString<InvalidCommentException>(content, nameof(Content));
            Guard.ForStringLength<InvalidCommentException>(content, ContentMinLength, ContentMaxLength, nameof(Content));
        }

        private static void ValidateAuthorId(Guid authorId)
        {
            Guard.AgainstEmptyString<InvalidCommentException>(authorId.ToString(), nameof(AuthorId));
        }

        private static void ValidateArticleId(ArticleId articleId)
        {
            Guard.AgainstDefault<InvalidCommentException, ArticleId>(articleId, nameof(ArticleId));
        }
    }
}
