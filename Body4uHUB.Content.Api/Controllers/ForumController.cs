using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Application.Commands.Forum.CreateForumPost;
using Body4uHUB.Content.Application.Commands.Forum.CreateForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.DeleteForumPost;
using Body4uHUB.Content.Application.Commands.Forum.DeleteForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.EditForumPost;
using Body4uHUB.Content.Application.Commands.Forum.EditForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.LockForumTopic;
using Body4uHUB.Content.Application.Commands.Forum.UnlockForumTopic;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Forum;
using Body4uHUB.Content.Application.Queries.Forum.GetAllForumTopics;
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
    [Authorize]
    [Route("api/forums")]
    public class ForumController : ApiController
    {
        /// <summary>
        /// Create a new forum topic
        /// </summary>
        [HttpPost("topics")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateForumTopic([FromBody] CreateForumTopicCommand command)
        {
            command = command with { AuthorId = User.GetUserId() };

            var result = await Mediator.Send(command);

            return HandleResult(result, id => new { topicId = id });
        }

        /// <summary>
        /// Delete forum topic (Admin only)
        /// </summary>
        [HttpDelete("topics/{topicId}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteForumTopic(Guid topicId)
        {
            var result = await Mediator.Send(new DeleteForumTopicCommand(topicId));
            return HandleResult(result);
        }

        /// <summary>
        /// Edit forum topic title (Author or Admin only)
        /// </summary>
        [HttpPut("topics/{topicId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditForumTopic(Guid topicId, [FromBody] EditForumTopicCommand command)
        {
            command = command with
            {
                TopicId = topicId,
                AuthContext = AuthorizationContext.Create(
                    User.GetUserId(),
                    User.IsAdmin()
                )
            };
            
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Get all forum topics with pagination
        /// </summary>
        [HttpGet("topics")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ForumTopicDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTopics([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] bool includeDeleted = false)
        {
            var result = await Mediator.Send(new GetAllForumTopicsQuery(skip, take, includeDeleted));
            return HandleResult(result);
        }

        /// <summary>
        /// Get forum topic by ID
        /// </summary>
        [HttpGet("topics/{topicId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ForumTopicDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetForumTopic(Guid topicId)
        {
            var result = await Mediator.Send(new GetForumTopicByIdQuery(topicId));
            return HandleResult(result);
        }

        /// <summary>
        /// Lock forum topic (Admin only)
        /// </summary>
        [HttpPost("topics/{topicId}/lock")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> LockForumTopic(Guid topicId)
        {
            var result = await Mediator.Send(new LockForumTopicCommand(topicId));
            return HandleResult(result);
        }

        /// <summary>
        /// Unlock forum topic (Admin only)
        /// </summary>
        [HttpPost("topics/{topicId}/unlock")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UnlockForumTopic(Guid topicId)
        {
            var result = await Mediator.Send(new UnlockForumTopicCommand(topicId));
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new post in a forum topic
        /// </summary>
        [HttpPost("topics/{topicId}/posts")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateForumPost(Guid topicId, [FromBody] CreateForumPostCommand command)
        {
            command = command with
            {
                TopicId = topicId,
                AuthorId = User.GetUserId()
            };

            var result = await Mediator.Send(command);
            return HandleResult(result, id => new { postId = id });
        }


        /// <summary>
        /// Delete forum post (Author or Admin only)
        /// </summary>
        [HttpDelete("topics/{topicId}/posts/{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteForumPost(Guid topicId, Guid postId)
        {
            var command = new DeleteForumPostCommand
            {
                PostId = postId,
                TopicId = topicId,
                AuthContext = AuthorizationContext.Create(
                    User.GetUserId(),
                    User.IsAdmin()
                )
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Edit forum post (Author or Admin only)
        /// </summary>
        [HttpPut("topics/{topicId}/posts/{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditForumPost(Guid topicId, Guid postId, [FromBody] EditForumPostCommand command)
        {
            command = command with
            {
                PostId = postId,
                TopicId = topicId,
                AuthContext = AuthorizationContext.Create(
                    User.GetUserId(),
                    User.IsAdmin()
                )
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
