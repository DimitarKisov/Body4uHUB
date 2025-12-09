using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Application.Commands.Forum;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Forum;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        [HttpPost("createTopic")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateForumTopic([FromBody] CreateForumTopicCommand command)
        {
            command.AuthorId = User.GetUserId();

            var result = await Mediator.Send(command);

            return HandleResult(result, id => new { topicId = id });
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
            var result = await Mediator.Send(new GetForumTopicByIdQuery { TopicId = topicId });
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new post in a forum topic
        /// </summary>
        [HttpPost("topics/{topicId}/createPost")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateForumPost(Guid topicId, [FromBody] CreateForumPostCommand command)
        {
            command.TopicId = topicId;
            command.AuthorId = User.GetUserId();

            var result = await Mediator.Send(command);
            return HandleResult(result, id => new { postId = id });
        }
    }
}
