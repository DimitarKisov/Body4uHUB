using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Domain.Models
{
    public class ForumTopic : Entity<Guid>, IAggregateRoot
    {
        private readonly List<ForumPost> _posts = new();

        public string Title { get; private set; }
        public int ViewCount { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool IsLocked { get; private set; }
        public Guid AuthorId { get; private set; }
        public IReadOnlyCollection<ForumPost> Posts => _posts.AsReadOnly();

        private ForumTopic()
            : base(new Guid())
        {
        }

        private ForumTopic(string title, Guid authorId)
            : base(Guid.NewGuid())
        {
            Title = title;
            AuthorId = authorId;
            ViewCount = 0;
            IsDeleted = false;
            IsLocked = false;
        }

        public static ForumTopic Create(string title, Guid authorId)
        {
            Validate(title, authorId);
            return new ForumTopic(title, authorId);
        }

        public void AddPost(ForumPost post)
        {
            _posts.Add(post);
        }

        public void UpdateTitle(string title)
        {
            ValidateTitle(title);
            Title = title;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        public void Lock()
        {
            if (IsLocked)
            {
                throw new InvalidForumTopicException(ForumTopicLocked);
            }

            IsLocked = true;
        }

        public void Unlock()
        {
            if (!IsLocked)
            {
                throw new InvalidForumTopicException(ForumTopicUnlocked);
            }

            IsLocked = false;
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }

        private static void Validate(string title, Guid authorId)
        {
            ValidateTitle(title);
            ValidateAuthorId(authorId);
        }

        private static void ValidateTitle(string title)
        {
            Guard.AgainstEmptyString<InvalidForumTopicException>(title, nameof(title));
            Guard.ForStringLength<InvalidForumTopicException>(title, TitleMinLength, TitleMaxLength, nameof(title));
        }

        private static void ValidateAuthorId(Guid authorId)
        {
            Guard.AgainstEmptyGuid<InvalidForumTopicException>(authorId, nameof(authorId));
        }
    }
}
