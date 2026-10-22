using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Commands.Articles.EditComment;

public record EditCommentCommand(int ArticleId, Guid CommentId, string Content, AuthorizationContext AuthContext) : IRequest<Result>;

internal sealed class EditCommentCommandHandler(
    IArticleRepository articleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<EditCommentCommand, Result>
{
    private readonly IArticleRepository _articleRepository = articleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetWithCommentsByIdAsync(request.ArticleId, cancellationToken);
        if (article is null)
        {
            return Result.ResourceNotFound(ArticleNotFound);
        }

        article.EditComment(
            request.CommentId,
            request.Content,
            request.AuthContext.CurrentUserId,
            request.AuthContext.IsAdmin);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
