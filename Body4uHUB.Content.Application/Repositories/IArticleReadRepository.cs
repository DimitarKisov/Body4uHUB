using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles.GetById;
using Body4uHUB.Shared.Application;

namespace Body4uHUB.Content.Application.Repositories;

public interface IArticleReadRepository
{
    Task<PagedResult<ArticleSummaryDto>> GetAllArticlesAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<ArticleSummaryDto>> GetArticlesByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<ArticleSummaryDto>> GetMyArticlesAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<GetArticleByIdResponse> GetWithCommentsByIdAsync(int id, CancellationToken cancellationToken = default);
}
