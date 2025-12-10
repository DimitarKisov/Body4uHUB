using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Application.Commands.Bookmarks.Commands.AddBookmark;
using Body4uHUB.Content.Application.Commands.Bookmarks.Commands.RemoveBookmark;
using Body4uHUB.Content.Application.Commands.Bookmarks.Queries.GetUserBookmarks;
using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Body4uHUB.Content.Api.Controllers
{
    [Route("api/bookmarks")]
    public class BookmarkController : ApiController
    {
        /// <summary>
        /// Add article to bookmarks
        /// </summary>
        [HttpPost("articles/{articleId}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddBookmark(int articleId)
        {
            var command = new AddBookmarkCommand
            {
                ArticleId = articleId,
                UserId = User.GetUserId()
            };

            var result = await Mediator.Send(command);
            return HandleResult(result, id => new { bookmarkId = id });
        }

        /// <summary>
        /// Get all bookmarks for current user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookmarkDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBookmarks([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var userId = User.GetUserId();
            var command = new GetUserBookmarksQuery
            {
                UserId = userId,
                Skip = skip,
                Take = take
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove article from bookmarks
        /// </summary>
        [HttpDelete("articles/{articleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RemoveBookmark(int articleId)
        {
            var userId = User.GetUserId();
            var command = new RemoveBookmarkCommand
            {
                UserId = userId,
                ArticleId = articleId
            };

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
