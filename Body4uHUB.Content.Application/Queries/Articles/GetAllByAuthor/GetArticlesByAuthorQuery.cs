using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor
{
    public record GetArticlesByAuthorQuery(Guid AuthorId, int Page, int PageSize): IRequest<Result<PagedResult<GetArticlesByAuthorResponse>>>;

    internal sealed class GetArticlesByAuthorQueryHandler : IRequestHandler<GetArticlesByAuthorQuery, Result<PagedResult<GetArticlesByAuthorResponse>>>
    {
        private readonly IArticleReadRepository _articleReadRepository;

        public GetArticlesByAuthorQueryHandler(IArticleReadRepository articleReadRepository)
        {
            _articleReadRepository = articleReadRepository;
        }

        public async Task<Result<PagedResult<GetArticlesByAuthorResponse>>> Handle(GetArticlesByAuthorQuery request, CancellationToken cancellationToken)
        {
            var articles = await _articleReadRepository.GetArticlesByAuthorAsync(request.AuthorId, request.Page, request.PageSize, cancellationToken);

            return Result.Success(articles);
        }
    }
}
