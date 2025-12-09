using Body4uHUB.Identity.Application.Commands.AddUserRoles;
using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Queries.GetAllRoles;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Identity.Api.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/roles")]
    public class RoleController : ApiController
    {
        /// <summary>
        /// Add roles to a user (Admin only)
        /// </summary>
        [HttpPost("users/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddRolesToUser(Guid userId, [FromBody] AddUserRolesCommand command)
        {
            command.UserId = userId;
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Get all available roles (Admin only)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await Mediator.Send(new GetAllRolesQuery());
            return Ok(roles);
        }
    }
}