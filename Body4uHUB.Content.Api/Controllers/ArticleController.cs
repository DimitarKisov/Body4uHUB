namespace Body4uHUB.Content.Api.Controllers
{
    using Body4uHUB.Content.Api.Extensions;
    using Body4uHUB.Content.Application.Commands.Articles.Create;
    using Body4uHUB.Content.Application.Commands.Articles.Edit;
    using Body4uHUB.Content.Application.Commands.Articles.Publish;
    using Body4uHUB.Content.Application.Commands.Comments.Create;
    using Body4uHUB.Content.Application.Commands.Comments.Delete;
    using Body4uHUB.Content.Application.DTOs;
    using Body4uHUB.Content.Application.Queries.Articles;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [Route("api/article")]
    public class ArticleController : ApiController
    {
        /// <summary>
        /// Create a new article (Trainers and Admins only)
        /// </summary>
        [HttpPost("create")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateArticle(CreateArticleCommand command)
        {
            var articleId = await Mediator.Send(command);
            return Ok(articleId);
        }

        /// <summary>
        /// Get article by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticle(int id)
        {
            var article = await Mediator.Send(new GetArticleByIdQuery { Id = id });
            return Ok(article);
        }

        /// <summary>
        /// Edit article (Trainers and Admins only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditArticle(int id, [FromBody] EditArticleCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Publish article (Trainers and Admins only)
        /// </summary>
        [HttpPost("{id}/publish")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PublishArticle(int id)
        {
            await Mediator.Send(new PublishArticleCommand { Id = id });
            return NoContent();
        }

        /// <summary>
        /// Add a comment to an article
        /// </summary>
        [HttpPost("{articleId}/comments")]
        [Authorize]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddComment(int articleId, [FromBody] CreateCommentCommand command)
        {
            command.AuthorId = User.GetUserId();
            command.ArticleId = Domain.ValueObjects.ArticleId.Create(articleId);

            var commentId = await Mediator.Send(command);
            return Ok(new { commentId = commentId.Value });
        }

        /// <summary>
        /// Delete a comment from an article (soft delete)
        /// </summary>
        [HttpDelete("{articleId}/comments/{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(int articleId, int commentId)
        {
            await Mediator.Send(new DeleteCommentCommand
            {
                Id = commentId,
                ArticleId = articleId
            });
            return NoContent();
        }
    }
}