using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Api.Models.Articles;
using Body4uHUB.Content.Application.Commands.Articles.Archive;
using Body4uHUB.Content.Application.Commands.Articles.Create;
using Body4uHUB.Content.Application.Commands.Articles.CreateComment;
using Body4uHUB.Content.Application.Commands.Articles.Delete;
using Body4uHUB.Content.Application.Commands.Articles.DeleteComment;
using Body4uHUB.Content.Application.Commands.Articles.Edit;
using Body4uHUB.Content.Application.Commands.Articles.Publish;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles;
using Body4uHUB.Content.Application.Queries.Articles.GetAll;
using Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor;
using Body4uHUB.Shared.Api;
using Body4uHUB.Shared.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Body4uHUB.Content.Api.Controllers
{
    [Route("api/articles")]
    public class ArticleController : ApiController
    {
        /// <summary>
        /// Archive article (Author or Admin only)
        /// </summary>
        [HttpPost("{id:int}/archive")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ArchiveArticle(Guid id)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var result = await Mediator.Send(new ArchiveArticleCommand(id, authContext));

            return HandleResult(result);
        }

        /// <summary>
        /// Create a new article (Trainers and Admins only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(CreateArticleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest request)
        {
            var command = new CreateArticleCommand(
                request.Title,
                request.Content,
                User.GetUserId());

            var result = await Mediator.Send(command);

            return HandleResult(result, id => new { articleId = id });
        }

        /// <summary>
        /// Delete article (Author or Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var result = await Mediator.Send(new DeleteArticleCommand(id, authContext));

            return HandleResult(result);
        }

        /// <summary>
        /// Get all published articles by a specific author
        /// </summary>
        [HttpGet("author/{authorId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<GetArticlesByAuthorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArticlesByAuthor(Guid authorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await Mediator.Send(new GetArticlesByAuthorQuery(authorId, page, pageSize));

            return HandleResult(result);
        }

        /// <summary>
        /// Get all published articles with pagination
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<GetAllArticlesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllArticles([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await Mediator.Send(new GetAllArticlesQuery(page, size));

            return HandleResult(result);
        }

        /// <summary>
        /// Get article by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetArticle(int id)
        {
            var result = await Mediator.Send(new GetArticleByIdQuery(id));

            return HandleResult(result);
        }

        /// <summary>
        /// Edit article (Author or Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditArticle(int id, [FromBody] EditArticleRequest request)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var command = new EditArticleCommand(id, request.Title, request.Content, authContext);
            var result = await Mediator.Send(command);

            return HandleResult(result);
        }

        /// <summary>
        /// Publish article (Author or Admin only)
        /// </summary>
        [HttpPost("{id}/publish")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PublishArticle(int id)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var result = await Mediator.Send(new PublishArticleCommand(id, authContext));

            return HandleResult(result);
        }

        /// <summary>
        /// Add a comment to an article
        /// </summary>
        [HttpPost("{articleId:int}/comments")]
        [Authorize]
        [ProducesResponseType(typeof(CreateCommentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddComment(int articleId, [FromBody] CreateCommentRequest request)
        {
            var command = new CreateCommentCommand(request.Content, articleId, User.GetUserId(), request.ParentCommentId);
            var result = await Mediator.Send(command);

            return HandleResult(result, id => new { commentId = id });
        }

        /// <summary>
        /// Delete a comment from an article (Author or Admin only)
        /// </summary>
        [HttpDelete("{articleId}/comments/{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteComment(int articleId, Guid commentId)
        {
            var result = await Mediator.Send(new DeleteCommentCommand(commentId, articleId)
            {
                AuthContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin())
            });

            return HandleResult(result);
        }
    }
}