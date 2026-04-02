using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Archive
{
    public record ArchiveArticleCommand(int Id)
        : IRequest<Result>
    {
        [JsonIgnore]
        public AuthorizationContext AuthContext { get; init; }
    }

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
            var article = await _articleRepository.GetByNumberAsync(request.Id, cancellationToken);
            if (article == null)
            {
                return Result.ResourceNotFound(ArticleNotFound);
            }

            if (!request.AuthContext.IsAdmin && article.AuthorId != request.AuthContext.CurrentUserId)
            {
                return Result.Forbidden(ArticleEditForbidden);
            }

            article.Archive();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
