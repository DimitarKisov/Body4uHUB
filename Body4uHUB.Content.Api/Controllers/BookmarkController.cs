using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Application.Commands.Bookmarks.AddBookmark;
using Body4uHUB.Content.Application.Commands.Bookmarks.RemoveBookmark;
using Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks;
using Body4uHUB.Shared.Api;
using Body4uHUB.Shared.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Body4uHUB.Content.Api.Controllers
{
    [Authorize]
    [Route("api/bookmarks")]
    public class BookmarkController : ApiController
    {
        /// <summary>
        /// Add article to bookmarks
        /// </summary>
        [HttpPost("articles/{articleId:int}")]
        [ProducesResponseType(typeof(AddBookmarkResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddBookmark(int articleId)
        {
            var command = new AddBookmarkCommand(User.GetUserId(), articleId);

            var result = await Mediator.Send(command);
            return HandleCreatedResult(result, id => new { bookmarkId = id });
        }

        /// <summary>
        /// Get all bookmarks for current user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<GetUserBookmarksResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBookmarks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var command = new GetUserBookmarksQuery(User.GetUserId(), page, pageSize);

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove article from bookmarks
        /// </summary>
        [HttpDelete("articles/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RemoveBookmark(int articleId)
        {
            var command = new RemoveBookmarkCommand(User.GetUserId(), articleId);

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
