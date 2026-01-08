using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Archive
{
    public class ArchiveArticleCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }

        internal class ArchiveArticleCommandHandler : IRequestHandler<ArchiveArticleCommand, Result>
        {
            private readonly IArticleRepository _articleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public ArchiveArticleCommandHandler(
                IArticleRepository articleRepository,
                IUnitOfWork unitOfWork)
            {
                _articleRepository = articleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ArchiveArticleCommand request, CancellationToken cancellationToken)
            {
                var article = await _articleRepository.GetByIdAsync(ArticleId.Create(request.Id), cancellationToken);
                if (article == null)
                {
                    return Result.ResourceNotFound(ArticleNotFound);
                }

                if (!request.IsAdmin && article.AuthorId != request.CurrentUserId)
                {
                    return Result.Forbidden(ArticleEditForbidden);
                }

                article.Archive();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
