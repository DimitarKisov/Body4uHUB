namespace Body4uHUB.Content.Api.Controllers
{
    using Body4uHUB.Content.Application.Commands.Articles.Create;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class ArticlesController : ApiController
    {
        /// <summary>
        /// Create a new article (Trainers and Admins only)
        /// </summary>
        [HttpPost]
        //[Authorize(Policy = "TrainerOrAdmin")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateArticle(CreateArticleCommand command)
        {
            var articleId = await Mediator.Send(command);
            return Ok(articleId);
        }
    }
}