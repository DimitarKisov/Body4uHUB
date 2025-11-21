using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Identity.Application.Commands.EditUser;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Identity.Api.Controllers
{
    [Authorize]
    [Route("api/user")]
    public class UserController : ApiController
    {
        /// <summary>
        /// Edit current user profile
        /// </summary>
        [HttpPut("edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditUser(EditUserCommand command)
        {
            command.Id = User.GetUserId();
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
