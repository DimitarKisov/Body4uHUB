using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Identity.Application.Commands.EditUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Identity.Api.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        [HttpPut]
        [Route(nameof(EditUser))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditUser(EditUserCommand command)
        {
            command.Id = User.GetUserId();
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
