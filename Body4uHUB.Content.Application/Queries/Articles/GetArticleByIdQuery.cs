using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Queries.Articles
{
    public class GetArticleByIdQuery : IRequest<Result<ArticleDto>>
    {
        public int Id { get; set; }

        internal class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, Result<ArticleDto>>
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

            public async Task<Result<ArticleDto>> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
            {

                var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);
                if (article == null)
                {
                    return Result.ResourceNotFound<ArticleDto>(ArticleNotFound);
                }

                article.IncrementViewCount();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var articleDto = new ArticleDto
                {
                    Id = article.Id.Value,
                    Title = article.Title,
                    Content = article.Content,
                    AuthorId = article.AuthorId,
                    Status = article.Status.Name,
                    PublishedAt = article.PublishedAt,
                    ViewCount = article.ViewCount,
                    CreatedAt = article.CreatedAt,
                    ModifiedAt = article.ModifiedAt,
                    Comments = article.Comments.Select(x => new CommentDto
                    {
                        Id = x.Id.Value,
                        AuthorId = x.AuthorId,
                        Content = x.Content,
                        CreatedAt = x.CreatedAt,
                        ModifiedAt = x.ModifiedAt
                    })
                    .ToList()
                };

                return Result.Success(articleDto);
            }
        }
    }
}