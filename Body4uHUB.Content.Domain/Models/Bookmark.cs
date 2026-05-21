using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

namespace Body4uHUB.Content.Domain.Models
{
    public class Bookmark : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public int ArticleId { get; private set; }

        private Bookmark()
            : base()
        {
        }

        private Bookmark(Guid userId, int articleId)
            : base(Guid.NewGuid())
        {
            UserId = userId;
            ArticleId = articleId;
        }

        public static Bookmark Create(Guid userId, int articleId)
        {
            Validate(userId, articleId);
            return new Bookmark(userId, articleId);
        }

        private static void Validate(Guid userId, int articleId)
        {
            ValidateUserId(userId);
            ValidateArticleId(articleId);
        }

        private static void ValidateUserId(Guid userId)
        {
            Guard.AgainstEmptyGuid<InvalidBookmarkException>(userId, nameof(userId));
        }

        private static void ValidateArticleId(int articleId)
        {
            Guard.AgainstNegativeAndZero<InvalidBookmarkException>(articleId, nameof(articleId));
        }
    }
}
