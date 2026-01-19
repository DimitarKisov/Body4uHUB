using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;
using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumPostConstants;

namespace Body4uHUB.Content.Domain.Models
{
    public class ForumPost : Entity<Guid>
    {
        public string Content { get; private set; }
        public Guid AuthorId { get; private set; }
        public bool IsDeleted { get; private set; }

        private ForumPost()
            : base(Guid.Empty)
        {
        }

        private ForumPost(string content, Guid authorId)
            : base(Guid.Empty)
        {
            Content = content;
            AuthorId = authorId;
            IsDeleted = false;
        }

        public static ForumPost Create(string content, Guid authorId)
        {
            Validate(content, authorId);
            return new ForumPost(content, authorId);
        }

        public void UpdateContent(string content)
        {
            if (IsDeleted)
            {
                throw new InvalidForumPostException(ForumPostDeleted);
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
            Guard.AgainstEmptyString<InvalidForumPostException>(content, nameof(content));
            Guard.ForStringLength<InvalidForumPostException>(content, ContentMinLength, ContentMaxLength, nameof(content));
        }

        private static void ValidateAuthorId(Guid authorId)
        {
            Guard.AgainstEmptyGuid<InvalidForumPostException>(authorId, nameof(authorId));
        }
    }
}
