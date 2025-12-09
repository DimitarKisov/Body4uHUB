using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAll
{
    public class GetAllArticlesQuery : IRequest<Result<IEnumerable<ArticleDto>>>
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;

        internal class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, Result<IEnumerable<ArticleDto>>>
        {
            private readonly IArticleReadRepository _articleReadRepository;

            public GetAllArticlesQueryHandler(IArticleReadRepository articleReadRepository)
            {
                _articleReadRepository = articleReadRepository;
            }

            public async Task<Result<IEnumerable<ArticleDto>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
            {
                var articles = await _articleReadRepository.GetAllArticlesAsync(request.Skip, request.Take, cancellationToken);

                return Result.Success(articles);
            }
        }
    }
}
