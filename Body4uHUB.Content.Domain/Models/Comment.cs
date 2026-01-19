using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

namespace Body4uHUB.Content.Domain.Models
{
    public class Comment : Entity<Guid>
    {
        public string Content { get; private set; }
        public Guid AuthorId { get; private set; }
        public Guid? ParentCommentId { get; private set; }
        public bool IsDeleted { get; private set; }

        private Comment()
            : base()
        {
        }

        private Comment(string content, Guid authorId, Guid? parentCommentId)
            : base(Guid.NewGuid())
        {
            Content = content;
            AuthorId = authorId;
            ParentCommentId = parentCommentId;
            IsDeleted = false;
        }

        public static Comment Create(string content, Guid authorId, Guid? parentCommentId)
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
