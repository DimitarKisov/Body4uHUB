using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ForumTopicConstants;

namespace Body4uHUB.Content.Domain.Models
{
    public class ForumTopic : AggregateRoot<int>
    {
        private readonly List<ForumPost> _posts = new();

        public string Title { get; private set; }
        public int ViewCount { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool IsLocked { get; private set; }
        public Guid AuthorId { get; private set; }
        public IReadOnlyCollection<ForumPost> Posts => _posts.AsReadOnly();

        private ForumTopic()
            : base()
        {
        }

        private ForumTopic(string title, Guid authorId)
            : base()
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

        public void Edit(string title, Guid requesterId, bool isAdmin)
        {
            if (IsDeleted)
            {
                throw new InvalidForumTopicException(ForumTopicDeleted);
            }

            EnsureCanBeModifiedBy(requesterId, isAdmin);

            ValidateTitle(title);
            Title = title;
        }

        public int AddPost(string content, Guid authorId)
        {
            if (IsLocked)
            {
                throw new InvalidForumTopicException(ForumTopicLocked);
            }

            var post = ForumPost.Create(content, authorId);
            _posts.Add(post);

            return post.Id;
        }

        public void EditPost(int postId, string content, Guid requesterId, bool isAdmin)
        {
            var post = _posts.FirstOrDefault(x => x.Id == postId);
            if (post == null)
            {
                throw new DomainNotFoundException(ForumPostNotFound);
            }

            if (!isAdmin && post.AuthorId != requesterId)
            {
                throw new DomainAuthorizationException(ForumPostEditForbidden);
            }

            post.UpdateContent(content);
        }

        public void DeletePost(int postId, Guid requesterId, bool isAdmin)
        {
            var post = _posts.FirstOrDefault(x => x.Id == postId);
            if (post == null)
            {
                throw new DomainNotFoundException(ForumPostNotFound);
            }

            if (!isAdmin && post.AuthorId != requesterId)
            {
                throw new DomainAuthorizationException(ForumPostDeleteForbidden);
            }

            post.MarkAsDeleted();
        }

        public void Delete(Guid requesterId, bool isAdmin)
        {
            EnsureCanBeModifiedBy(requesterId, isAdmin);

            if (IsDeleted)
            {
                throw new InvalidForumTopicException(ForumTopicDeleted);
            }

            MarkAsDeleted();
        }

        public void UpdateTitle(string title)
        {
            ValidateTitle(title);
            Title = title;
        }

        private void MarkAsDeleted()
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

        private void EnsureCanBeModifiedBy(Guid requesterId, bool isAdmin)
        {
            if (!isAdmin && AuthorId != requesterId)
            {
                throw new DomainAuthorizationException(ForumTopicModifyForbidden);
            }
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
