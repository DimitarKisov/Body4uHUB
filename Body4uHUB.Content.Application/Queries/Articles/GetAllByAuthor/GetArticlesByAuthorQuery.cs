using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor
{
    public record GetArticlesByAuthorQuery(Guid AuthorId): IRequest<Result<IEnumerable<GetArticlesByAuthorResponse>>>;

    internal sealed class GetArticlesByAuthorQueryHandler : IRequestHandler<GetArticlesByAuthorQuery, Result<IEnumerable<GetArticlesByAuthorResponse>>>
    {
        private readonly IArticleReadRepository _articleReadRepository;

        public GetArticlesByAuthorQueryHandler(IArticleReadRepository articleReadRepository)
        {
            _articleReadRepository = articleReadRepository;
        }

        public async Task<Result<IEnumerable<GetArticlesByAuthorResponse>>> Handle(GetArticlesByAuthorQuery request, CancellationToken cancellationToken)
        {
            var articles = await _articleReadRepository.GetArticlesByAuthorAsync(request.AuthorId, cancellationToken);

            return Result.Success(articles);
        }
    }
}
