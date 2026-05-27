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
        /// Get all available roles (Admin only)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await Mediator.Send(new GetAllRolesQuery());
            return Ok(roles);
        }
    }
}
