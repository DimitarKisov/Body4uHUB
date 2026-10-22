using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.Edit;

public record EditArticleCommand(int ArticleId, string Title, string Content, AuthorizationContext AuthContext): IRequest<Result>;

internal sealed class EditArticleCommandHandler(
    IArticleRepository articleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<EditArticleCommand, Result>
{
    private readonly IArticleRepository _articleRepository = articleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(EditArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(request.ArticleId, cancellationToken);
        if (article is null)
        {
            return Result.ResourceNotFound(ArticleNotFound);
        }

        article.Edit(request.Title, request.Content, request.AuthContext.CurrentUserId, request.AuthContext.IsAdmin);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
