using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Api.Models.Articles;
using Body4uHUB.Content.Application.Commands.Articles.Archive;
using Body4uHUB.Content.Application.Commands.Articles.Create;
using Body4uHUB.Content.Application.Commands.Articles.CreateComment;
using Body4uHUB.Content.Application.Commands.Articles.Delete;
using Body4uHUB.Content.Application.Commands.Articles.DeleteComment;
using Body4uHUB.Content.Application.Commands.Articles.Edit;
using Body4uHUB.Content.Application.Commands.Articles.EditComment;
using Body4uHUB.Content.Application.Commands.Articles.Publish;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles.GetAll;
using Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor;
using Body4uHUB.Content.Application.Queries.Articles.GetById;
using Body4uHUB.Content.Application.Queries.Articles.GetMine;
using Body4uHUB.Shared.Api;
using Body4uHUB.Shared.Api.Extensions;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Application.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Body4uHUB.Content.Api.Controllers;

[Route("api/articles")]
public class ArticleController : ApiController
{
    /// <summary>
    /// Archive article (Author or Admin only)
    /// </summary>
    [HttpPost("{id:int}/archive")]
    [Authorize(Policy = AuthorizationPolicies.TrainerOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ArchiveArticle(int id, CancellationToken cancellationToken)
    {
        var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
        var result = await Mediator.Send(new ArchiveArticleCommand(id, authContext), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Create a new article (Trainers and Admins only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.TrainerOrAdmin)]
    [ProducesResponseType(typeof(CreateArticleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateArticleCommand(
            request.Title,
            request.Content,
            request.Category,
            User.GetUserId());

        var result = await Mediator.Send(command, cancellationToken);

        return HandleCreatedResult(result, id => new { articleId = id });
    }

    /// <summary>
    /// Delete article (Author or Admin only)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.TrainerOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteArticle(int id, CancellationToken cancellationToken)
    {
        var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
        var result = await Mediator.Send(new DeleteArticleCommand(id, authContext), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get all published articles by a specific author with pagination
    /// </summary>
    [HttpGet("author/{authorId:Guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<ArticleSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetArticlesByAuthor(Guid authorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetArticlesByAuthorQuery(authorId, page, pageSize), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get the current user's articles in every status except deleted, most recently changed first (Trainers and Admins only)
    /// </summary>
    [HttpGet("mine")]
    [Authorize(Policy = AuthorizationPolicies.TrainerOrAdmin)]
    [ProducesResponseType(typeof(PagedResult<ArticleSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyArticles([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetMyArticlesQuery(User.GetUserId(), page, pageSize), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get all published articles with pagination
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<ArticleSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllArticles([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetAllArticlesQuery(page, pageSize), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get article by ID (drafts and archived articles are visible to their author and admins only)
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetArticleByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticle(int id, CancellationToken cancellationToken)
    {
        var authContext = User.Identity?.IsAuthenticated == true
            ? AuthorizationContext.Create(User.GetUserId(), User.IsAdmin())
            : null;

        var result = await Mediator.Send(new GetArticleByIdQuery(id, authContext, HttpContext.Connection.RemoteIpAddress?.ToString()), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Edit article (Author or Admin only)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.TrainerOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EditArticle(int id, [FromBody] EditArticleRequest request, CancellationToken cancellationToken)
    {
        var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
        var command = new EditArticleCommand(id, request.Title, request.Content, request.Category, authContext);
        var result = await Mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Publish article (Author or Admin only)
    /// </summary>
    [HttpPost("{id:int}/publish")]
    [Authorize(Policy = AuthorizationPolicies.TrainerOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PublishArticle(int id, CancellationToken cancellationToken)
    {
        var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
        var result = await Mediator.Send(new PublishArticleCommand(id, authContext), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Add a comment to an article
    /// </summary>
    [HttpPost("{articleId:int}/comments")]
    [Authorize]
    [EnableRateLimiting(RateLimitPolicies.CreateComment)]
    [ProducesResponseType(typeof(CreateCommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> AddComment(int articleId, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand(request.Content, articleId, User.GetUserId(), request.ParentCommentId);
        var result = await Mediator.Send(command, cancellationToken);

        return HandleCreatedResult(result, id => new { commentId = id });
    }

    /// <summary>
    /// Edit a comment on an article (Author or Admin only)
    /// </summary>
    [HttpPut("{articleId:int}/comments/{commentId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EditComment(int articleId, Guid commentId, [FromBody] EditCommentRequest request, CancellationToken cancellationToken)
    {
        var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
        var command = new EditCommentCommand(articleId, commentId, request.Content, authContext);
        var result = await Mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Delete a comment from an article (Author or Admin only)
    /// </summary>
    [HttpDelete("{articleId:int}/comments/{commentId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteComment(int articleId, Guid commentId, CancellationToken cancellationToken)
    {
        var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
        var result = await Mediator.Send(new DeleteCommentCommand(commentId, articleId, authContext), cancellationToken);

        return HandleResult(result);
    }
}
