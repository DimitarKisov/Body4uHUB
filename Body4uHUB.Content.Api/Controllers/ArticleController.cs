using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Application.Commands.Articles.Archive;
using Body4uHUB.Content.Application.Commands.Articles.Create;
using Body4uHUB.Content.Application.Commands.Articles.Delete;
using Body4uHUB.Content.Application.Commands.Articles.Edit;
using Body4uHUB.Content.Application.Commands.Articles.Publish;
using Body4uHUB.Content.Application.Commands.Comments.Create;
using Body4uHUB.Content.Application.Commands.Comments.Delete;
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
        [HttpPost("{id}/archive")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ArchiveArticle(int id)
        {
            var command = new ArchiveArticleCommand
            {
                Id = id,
                AuthContext = new AuthorizationContext
                {
                    CurrentUserId = User.GetUserId(),
                    IsAdmin = User.IsAdmin()
                }
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new article (Trainers and Admins only)
        /// </summary>
        [HttpPost("create")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleCommand command)
        {
            command.AuthorId = User.GetUserId();
            var result = await Mediator.Send(command);

            return HandleResult(result, id => new { articleId = id });
        }

        /// <summary>
        /// Delete article (Author or Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var command = new DeleteArticleCommand
            {
                Id = id,
                AuthContext = new AuthorizationContext
                {
                    CurrentUserId = User.GetUserId(),
                    IsAdmin = User.IsAdmin()
                }
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Get all articles by a specific author
        /// </summary>
        [HttpGet("author/{authorId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArticlesByAuthor(Guid authorId)
        {
            var result = await Mediator.Send(new GetArticlesByAuthorQuery { AuthorId = authorId });
            return HandleResult(result);
        }

        /// <summary>
        /// Get all published articles with pagination
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllArticles([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var result = await Mediator.Send(new GetAllArticlesQuery { Skip = skip, Take = take });
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
            var result = await Mediator.Send(new GetArticleByIdQuery { Id = id });
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
        public async Task<IActionResult> EditArticle(int id, [FromBody] EditArticleCommand command)
        {
            command.Id = id;
            command.AuthContext = new AuthorizationContext
            {
                CurrentUserId = User.GetUserId(),
                IsAdmin = User.IsAdmin()
            };

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
            var command = new PublishArticleCommand();

            command.Id = id;
            command.AuthContext = new AuthorizationContext
            {
                CurrentUserId = User.GetUserId(),
                IsAdmin = User.IsAdmin()
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Add a comment to an article
        /// </summary>
        [HttpPost("{articleId}/comments")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddComment(int articleId, [FromBody] CreateCommentCommand command)
        {
            command.AuthorId = User.GetUserId();
            command.ArticleId = Domain.ValueObjects.ArticleId.Create(articleId);

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
        public async Task<IActionResult> DeleteComment(int articleId, int commentId)
        {
            var command = new DeleteCommentCommand
            {
                Id = commentId,
                ArticleId = articleId,
                AuthContext = new AuthorizationContext
                {
                    CurrentUserId = User.GetUserId(),
                    IsAdmin = User.IsAdmin()
                }
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}