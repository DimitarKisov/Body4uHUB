using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles.GetAll;
using Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor;
using Body4uHUB.Shared.Application;

namespace Body4uHUB.Content.Application.Repositories
{
    public interface IArticleReadRepository
    {
        Task<PagedResult<GetAllArticlesResponse>> GetAllArticlesAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<GetArticlesByAuthorResponse>> GetArticlesByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<ArticleDto> GetByNumberAsync(int id, CancellationToken cancellationToken = default);
    }
}
