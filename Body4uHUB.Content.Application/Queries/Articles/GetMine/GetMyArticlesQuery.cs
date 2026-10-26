using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Articles.GetMine;

public record GetMyArticlesQuery(Guid AuthorId, int Page, int PageSize) : IRequest<Result<PagedResult<ArticleSummaryDto>>>;

internal sealed class GetMyArticlesQueryHandler(IArticleReadRepository articleReadRepository)
    : IRequestHandler<GetMyArticlesQuery, Result<PagedResult<ArticleSummaryDto>>>
{
    private readonly IArticleReadRepository _articleReadRepository = articleReadRepository;

    public async Task<Result<PagedResult<ArticleSummaryDto>>> Handle(GetMyArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _articleReadRepository.GetMyArticlesAsync(request.AuthorId, request.Page, request.PageSize, cancellationToken);

        return Result.Success(articles);
    }
}
