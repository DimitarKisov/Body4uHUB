using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

namespace Body4uHUB.Content.Domain.Models
{
    public class Bookmark : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public ArticleId ArticleId { get; private set; }

        private Bookmark()
            : base(default!)
        {
        }

        private Bookmark(Guid userId, ArticleId articleId)
            : base(default!)
        {
            UserId = userId;
            ArticleId = articleId;
        }

        public static Bookmark Create(Guid userId, ArticleId articleId)
        {
            Validate(userId, articleId);
            return new Bookmark(userId, articleId);
        }

        private static void Validate(Guid userId, ArticleId articleId)
        {
            ValidateUserId(userId);
            ValidateArticleId(articleId);
        }

        private static void ValidateUserId(Guid userId)
        {
            Guard.AgainstEmptyGuid<InvalidBookmarkException>(userId, nameof(userId));
        }

        private static void ValidateArticleId(ArticleId articleId)
        {
            Guard.AgainstDefault<InvalidBookmarkException, ArticleId>(articleId, nameof(articleId));
        }
    }
}
