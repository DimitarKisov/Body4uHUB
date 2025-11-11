using Body4uHUB.Content.Application.DTOs;

namespace Body4uHUB.Content.Application.Repositories
{
    public interface IArticleReadRepository
    {
        Task<IEnumerable<ArticleDto>> GetAllArticlesAsync(int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<ArticleDto>> GetArticlesByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
    }
}
