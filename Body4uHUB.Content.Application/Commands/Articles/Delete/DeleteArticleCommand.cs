using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Delete
{
    public record DeleteArticleCommand(int Id, AuthorizationContext AuthContext) : IRequest<Result>;

    internal sealed class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Result>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteArticleCommandHandler(
            IArticleRepository articleRepository,
            IUnitOfWork unitOfWork)
        {
            _articleRepository = articleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
        {
            var article = await _articleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (article is null)
            {
                return Result.ResourceNotFound(ArticleNotFound);
            }

            article.Delete(request.AuthContext.CurrentUserId, request.AuthContext.IsAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
