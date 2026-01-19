using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

namespace Body4uHUB.Content.Domain.Models
{
    public class Comment : Entity<CommentId>
    {
        public string Content { get; private set; }
        public Guid AuthorId { get; private set; }
        public CommentId ParentCommentId { get; private set; }
        public bool IsDeleted { get; private set; }

        private Comment()
            : base(default!)
        {
        }

        private Comment(string content, Guid authorId, CommentId parentCommentId = null)
            : base(default!)
        {
            Content = content;
            AuthorId = authorId;
            ParentCommentId = parentCommentId;
            IsDeleted = false;
        }

        public static Comment Create(string content, Guid authorId, CommentId parentCommentId = null)
        {
            Validate(content, authorId);
            return new Comment(content, authorId, parentCommentId);
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

        private static void Validate(string content, Guid authorId)
        {
            ValidateContent(content);
            ValidateAuthorId(authorId);
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
    }
}
