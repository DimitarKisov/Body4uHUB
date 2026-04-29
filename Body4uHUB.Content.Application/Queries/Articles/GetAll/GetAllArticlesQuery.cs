using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAll
{
    public record GetAllArticlesQuery(int Page, int Size): IRequest<Result<PagedResult<GetAllArticlesResponse>>>;

    internal sealed class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, Result<PagedResult<GetAllArticlesResponse>>>
    {
        private readonly IArticleReadRepository _articleReadRepository;

        public GetAllArticlesQueryHandler(IArticleReadRepository articleReadRepository)
        {
            _articleReadRepository = articleReadRepository;
        }

        public async Task<Result<PagedResult<GetAllArticlesResponse>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {
            var articles = await _articleReadRepository.GetAllArticlesAsync(request.Page, request.Size, cancellationToken);

            return Result.Success(articles);
        }
    }
}
