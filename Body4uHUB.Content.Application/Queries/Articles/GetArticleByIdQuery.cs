namespace Body4uHUB.Content.Application.Queries.Articles
{
    using Body4uHUB.Content.Application.DTOs;
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Shared;
    using Body4uHUB.Shared.Exceptions;
    using MediatR;

    using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

    public class GetArticleByIdQuery : IRequest<ArticleDto>
    {
        public int Id { get; set; }

        internal class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto>
        {
            private readonly IArticleRepository _articleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public GetArticleByIdQueryHandler(
                IArticleRepository articleRepository,
                IUnitOfWork unitOfWork)
            {
                _articleRepository = articleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<ArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
            {

                var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);
                if (article == null)
                {
                    throw new NotFoundException(ArticleNotFound);
                }

                article.IncrementViewCount();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ArticleDto
                {
                    Id = article.Id.Value,
                    Title = article.Title,
                    Content = article.Content,
                    AuthorId = article.AuthorId,
                    Status = article.Status.Name,
                    PublishedAt = article.PublishedAt,
                    ViewCount = article.ViewCount,
                    CreatedAt = article.CreatedAt,
                    ModifiedAt = article.ModifiedAt
                };
            }
        }
    }
}