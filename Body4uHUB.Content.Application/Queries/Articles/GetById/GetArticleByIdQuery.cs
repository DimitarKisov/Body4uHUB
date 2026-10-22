using Body4uHUB.Content.Application.Abstractions;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Application.Queries.Articles.GetById;

public record GetArticleByIdQuery(int Id, AuthorizationContext AuthContext, string VisitorIp) : IRequest<Result<GetArticleByIdResponse>>;

internal sealed class GetArticleByIdQueryHandler(
    IArticleRepository articleRepository,
    IArticleReadRepository articleReadRepository,
    IViewTracker viewTracker)
    : IRequestHandler<GetArticleByIdQuery, Result<GetArticleByIdResponse>>
{
    private readonly IArticleRepository _articleRepository = articleRepository;
    private readonly IArticleReadRepository _articleReadRepository = articleReadRepository;
    private readonly IViewTracker _viewTracker = viewTracker;

    public async Task<Result<GetArticleByIdResponse>> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        var article = await _articleReadRepository.GetWithCommentsByIdAsync(request.Id, cancellationToken);
        if (article == null)
        {
            return Result.ResourceNotFound<GetArticleByIdResponse>(ArticleNotFound);
        }

        var isPublished = article.Status == ArticleStatus.Published.Name;
        if (!isPublished && !CanViewUnpublished(article, request.AuthContext))
        {
            return Result.ResourceNotFound<GetArticleByIdResponse>(ArticleNotFound);
        }

        if (isPublished && await _viewTracker.TryRegisterViewAsync(ViewTarget.Article, request.Id, request.VisitorIp, cancellationToken))
        {
            var success = await _articleRepository.IncrementViewCountAsync(request.Id, cancellationToken);
            if (!success)
            {
                return Result.ResourceNotFound<GetArticleByIdResponse>(ArticleNotFound);
            }

            article = article with { ViewCount = article.ViewCount + 1 };
        }

        return Result.Success(article with { Comments = VisibleComments(article.Comments) });
    }

    private static bool CanViewUnpublished(GetArticleByIdResponse article, AuthorizationContext authContext)
    {
        return authContext is not null
            && (authContext.IsAdmin || authContext.CurrentUserId == article.AuthorId);
    }

    // A deleted comment stays (without content) only while it still has visible replies,
    // so the thread does not break; otherwise it is dropped.
    private static List<CommentDto> VisibleComments(ICollection<CommentDto> comments)
    {
        var visible = comments.ToList();

        // Removing a deleted comment can leave its deleted parent without replies,
        // so repeat until nothing more can be removed.
        while (RemoveDeletedLeaves(visible))
        {
        }

        return [.. visible.Select(x => x.IsDeleted ? x with { Content = null } : x)];
    }

    private static bool RemoveDeletedLeaves(List<CommentDto> comments)
    {
        var idsWithReplies = comments
            .Where(x => x.ParentCommentId.HasValue)
            .Select(x => x.ParentCommentId.Value)
            .ToHashSet();

        var removedCount = comments.RemoveAll(x => x.IsDeleted && !idsWithReplies.Contains(x.Id));

        return removedCount > 0;
    }
}
