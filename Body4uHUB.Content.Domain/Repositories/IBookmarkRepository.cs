using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IBookmarkRepository : IRepository<Bookmark>
    {
        void Add(Bookmark bookmark);
        Task<bool> ExistsAsync(Guid userId, Guid articleId, CancellationToken cancellationToken = default);
        Task<Bookmark> GetByUserAndArticleAsync(Guid userId, int articleNumber, CancellationToken cancellationToken = default);
        void Remove(Bookmark bookmark);
    }
}