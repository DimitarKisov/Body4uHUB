using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Queries.Articles.GetById
{
    public record GetArticleByIdQuery(int ArticleNumber): IRequest<Result<GetArticleByIdResponse>>;

    internal sealed class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, Result<GetArticleByIdResponse>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleReadRepository _articleReadRepository;

        public GetArticleByIdQueryHandler(
            IArticleRepository articleRepository,
            IArticleReadRepository articleReadRepository)
        {
            _articleRepository = articleRepository;
            _articleReadRepository = articleReadRepository;
        }

        public async Task<Result<GetArticleByIdResponse>> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            var article = await _articleReadRepository.GetByNumberAsync(request.ArticleNumber, cancellationToken);
            if (article == null)
            {
                return Result.ResourceNotFound<GetArticleByIdResponse>(ArticleNotFound);
            }

            var success = await _articleRepository.IncrementViewCountAsync(request.ArticleNumber, cancellationToken);
            if (!success)
            {
                return Result.ResourceNotFound<GetArticleByIdResponse>(ArticleNotFound);
            }

            return Result.Success(article);
        }
    }
}