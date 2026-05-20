using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Api.Models.Forum;
using Body4uHUB.Content.Application.Commands.Forum.CreateForumPost;
using Body4uHUB.Content.Application.Commands.Forum.CreateForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.DeleteForumPost;
using Body4uHUB.Content.Application.Commands.Forum.DeleteForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.EditForumPost;
using Body4uHUB.Content.Application.Commands.Forum.EditForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.LockForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.UnlockForumTopic;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Forum.GetAllForumTopics;
using Body4uHUB.Content.Application.Queries.Forum.GetById;
using Body4uHUB.Shared.Api;
using Body4uHUB.Shared.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Content.Api.Controllers
{
    [Route("api/forums")]
    public class ForumController : ApiController
    {
        /// <summary>
        /// Create a new forum topic (Trainers and Admins only)
        /// </summary>
        [HttpPost("topics")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(CreateForumTopicResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateForumTopic([FromBody] CreateForumTopicRequest request)
        {
            var command = new CreateForumTopicCommand(request.Title, User.GetUserId());
            var result = await Mediator.Send(command);

            return HandleResult(result, id => new { topicId = id });
        }

        /// <summary>
        /// Delete forum topic (Author or Admin only)
        /// </summary>
        [HttpDelete("topics/{topicId:int}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteForumTopic(int topicId)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var result = await Mediator.Send(new DeleteForumTopicCommand(topicId, authContext));

            return HandleResult(result);
        }

        /// <summary>
        /// Edit forum topic title (Author or Admin only)
        /// </summary>
        [HttpPut("topics/{topicId:int}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditForumTopic(int topicId, [FromBody] EditForumTopicRequest request)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var command = new EditForumTopicCommand(topicId, request.Title, authContext);
            var result = await Mediator.Send(command);

            return HandleResult(result);
        }

        /// <summary>
        /// Get all forum topics with pagination
        /// </summary>
        [HttpGet("topics")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<ForumTopicDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTopics([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool includeDeleted = false)
        {
            var result = await Mediator.Send(new GetAllForumTopicsQuery(page, pageSize, includeDeleted));

            return HandleResult(result);
        }

        /// <summary>
        /// Get forum topic by ID
        /// </summary>
        [HttpGet("topics/{topicId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GetForumTopicByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetForumTopic(int topicId)
        {
            var result = await Mediator.Send(new GetForumTopicByIdQuery(topicId));

            return HandleResult(result);
        }

        /// <summary>
        /// Lock forum topic (Admin only)
        /// </summary>
        [HttpPost("topics/{topicId:int}/lock")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> LockForumTopic(int topicId)
        {
            var result = await Mediator.Send(new LockForumTopicCommand(topicId));

            return HandleResult(result);
        }

        /// <summary>
        /// Unlock forum topic (Admin only)
        /// </summary>
        [HttpPost("topics/{topicId:int}/unlock")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UnlockForumTopic(int topicId)
        {
            var result = await Mediator.Send(new UnlockForumTopicCommand(topicId));

            return HandleResult(result);
        }

        /// <summary>
        /// Create a new post in a forum topic (Trainers and Admins only)
        /// </summary>
        [HttpPost("topics/{topicId:int}/posts")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(CreateForumPostResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateForumPost(int topicId, [FromBody] CreateForumPostRequest request)
        {
            var command = new CreateForumPostCommand(request.Content, topicId, User.GetUserId());
            var result = await Mediator.Send(command);

            return HandleResult(result, id => new { postId = id });
        }

        /// <summary>
        /// Delete forum post (Author or Admin only)
        /// </summary>
        [HttpDelete("topics/{topicId:int}/posts/{postId:int}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteForumPost(int topicId, int postId)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var command = new DeleteForumPostCommand(topicId, postId, authContext);
            var result = await Mediator.Send(command);

            return HandleResult(result);
        }

        /// <summary>
        /// Edit forum post (Author or Admin only)
        /// </summary>
        [HttpPut("topics/{topicId:int}/posts/{postId:int}")]
        [Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditForumPost(int topicId, int postId, [FromBody] EditForumPostRequest request)
        {
            var authContext = AuthorizationContext.Create(User.GetUserId(), User.IsAdmin());
            var command = new EditForumPostCommand(topicId, postId, request.Content, authContext);
            var result = await Mediator.Send(command);

            return HandleResult(result);
        }
    }
}
