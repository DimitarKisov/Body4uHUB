using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor;

public record GetArticlesByAuthorQuery(Guid AuthorId, int Page, int PageSize): IRequest<Result<PagedResult<ArticleSummaryDto>>>;

internal sealed class GetArticlesByAuthorQueryHandler(IArticleReadRepository articleReadRepository)
    : IRequestHandler<GetArticlesByAuthorQuery, Result<PagedResult<ArticleSummaryDto>>>
{
    private readonly IArticleReadRepository _articleReadRepository = articleReadRepository;

    public async Task<Result<PagedResult<ArticleSummaryDto>>> Handle(GetArticlesByAuthorQuery request, CancellationToken cancellationToken)
    {
        var articles = await _articleReadRepository.GetArticlesByAuthorAsync(request.AuthorId, request.Page, request.PageSize, cancellationToken);

        return Result.Success(articles);
    }
}
