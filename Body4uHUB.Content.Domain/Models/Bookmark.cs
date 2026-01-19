using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

namespace Body4uHUB.Content.Domain.Models
{
    public class Bookmark : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid ArticleId { get; private set; }
        public int ArticleNumber { get; private set; }

        private Bookmark()
            : base()
        {
        }

        private Bookmark(Guid userId, Guid articleId, int articleNumber)
            : base(Guid.NewGuid())
        {
            UserId = userId;
            ArticleId = articleId;
            ArticleNumber = articleNumber;
        }

        public static Bookmark Create(Guid userId, Guid articleId, int articleNumber)
        {
            Validate(userId, articleId, articleNumber);
            return new Bookmark(userId, articleId, articleNumber);
        }

        private static void Validate(Guid userId, Guid articleId, int articleNumber)
        {
            ValidateUserId(userId);
            ValidateArticleId(articleId);
            ValidateArticleNumber(articleNumber);
        }

        private static void ValidateUserId(Guid userId)
        {
            Guard.AgainstEmptyGuid<InvalidBookmarkException>(userId, nameof(userId));
        }

        private static void ValidateArticleId(Guid articleId)
        {
            Guard.AgainstEmptyGuid<InvalidBookmarkException>(articleId, nameof(articleId));
        }

        private static void ValidateArticleNumber(int articleNumber)
        {
            Guard.AgainstNegativeAndZero<InvalidBookmarkException>(articleNumber, nameof(articleNumber));
        }
    }
}
