using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAll;

public record GetAllArticlesQuery(int Page, int PageSize): IRequest<Result<PagedResult<ArticleSummaryDto>>>;

internal sealed class GetAllArticlesQueryHandler(IArticleReadRepository articleReadRepository)
    : IRequestHandler<GetAllArticlesQuery, Result<PagedResult<ArticleSummaryDto>>>
{
    private readonly IArticleReadRepository _articleReadRepository = articleReadRepository;

    public async Task<Result<PagedResult<ArticleSummaryDto>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _articleReadRepository.GetAllArticlesAsync(request.Page, request.PageSize, cancellationToken);

        return Result.Success(articles);
    }
}
