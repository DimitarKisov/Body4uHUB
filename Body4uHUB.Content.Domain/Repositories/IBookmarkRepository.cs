using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IBookmarkRepository : IRepository<Bookmark>
    {
        void Add(Bookmark bookmark);
        Task<bool> ExistsAsync(Guid userId, ArticleId articleId, CancellationToken cancellationToken = default);
        Task<Bookmark> GetByUserAndArticleAsync(Guid userId, ArticleId articleId, CancellationToken cancellationToken = default);
        void Remove(Bookmark bookmark);
    }
}