using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Publish;

public record PublishArticleCommand(int ArticleId, AuthorizationContext AuthContext) : IRequest<Result>;

internal sealed class PublishArticleCommandHandler(
    IArticleRepository articleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PublishArticleCommand, Result>
{
    private readonly IArticleRepository _articleRepository = articleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(PublishArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(request.ArticleId, cancellationToken);
        if (article == null)
        {
            return Result.ResourceNotFound(ArticleNotFound);
        }

        article.Publish(request.AuthContext.CurrentUserId, request.AuthContext.IsAdmin);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
