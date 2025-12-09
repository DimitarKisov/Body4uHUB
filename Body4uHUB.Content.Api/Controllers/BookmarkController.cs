using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Application.Commands.Bookmarks;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Body4uHUB.Content.Api.Controllers
{
    [Route("api/bookmark")]
    public class BookmarkController : ApiController
    {
        /// <summary>
        /// Add article to bookmarks
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddBookmark([FromBody] AddBookmarkCommand command)
        {
            command.UserId = User.GetUserId();

            var bookmarkId = await Mediator.Send(command);

            return Ok(new { bookmarkId });
        }

        /// <summary>
        /// Remove article from bookmarks
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveBookmark([FromQuery] int articleId)
        {
            var userId = User.GetUserId();

            await Mediator.Send(new RemoveBookmarkCommand
            {
                UserId = userId,
                ArticleId = articleId
            });

            return NoContent();
        }
    }
}
