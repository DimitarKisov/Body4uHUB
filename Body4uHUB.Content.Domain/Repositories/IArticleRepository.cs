using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        void Add(Article article);
        Task<bool> ExistsByIdAsync(ArticleId id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<Article> GetByIdAsync(ArticleId id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Article>> GetPublishedArticlesAsync(int skip, int take, CancellationToken cancellationToken = default);
    }
}
