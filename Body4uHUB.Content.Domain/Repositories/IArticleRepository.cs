using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        void Add(Article article);
        Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNumberAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<Article> GetWithCommentsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Article> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken = default);
    }
}
