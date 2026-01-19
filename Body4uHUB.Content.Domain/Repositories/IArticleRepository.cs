using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        void Add(Article article);
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNumberAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<Guid> GetArticleIdByNumberAsync(int number, CancellationToken cancellationToken = default);
        Task<Article> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Article> GetByNumberAsync(int id, CancellationToken cancellationToken = default);
        void Remove(Article article);
    }
}
