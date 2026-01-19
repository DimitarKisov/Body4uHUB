using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Create
{
    public class CreateArticleCommand : IRequest<Result<ArticleId>>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid AuthorId { get; set; }

        internal class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, Result<ArticleId>>
        {
            private readonly IArticleRepository _articleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateArticleCommandHandler(
                IArticleRepository articleRepository,
                IUnitOfWork unitOfWork)
            {
                _articleRepository = articleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<ArticleId>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
            {
                var articleExists = await _articleRepository.ExistsByTitleAsync(request.Title, cancellationToken);
                if (articleExists)
                {
                    return Result.Conflict<ArticleId>(string.Format(ArticleExists, request.Title));
                }

                var article = Article.Create(
                    request.Title,
                    request.Content,
                    request.AuthorId);

                _articleRepository.Add(article);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(article.Id);
            }
        }
    }
}