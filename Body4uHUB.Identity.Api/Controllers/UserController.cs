using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Identity.Api.Models.Roles;
using Body4uHUB.Identity.Api.Models.Users;
using Body4uHUB.Identity.Application.Commands.AddUserRoles;
using Body4uHUB.Identity.Application.Commands.ChangePassword;
using Body4uHUB.Identity.Application.Commands.CreateTrainer;
using Body4uHUB.Identity.Application.Commands.DeleteTrainer;
using Body4uHUB.Identity.Application.Commands.EditUser;
using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Queries.GetAllUsers;
using Body4uHUB.Identity.Application.Queries.GetUserById;
using Body4uHUB.Shared.Api;
using Body4uHUB.Shared.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Identity.Api.Controllers
{
    [Authorize]
    [Route("api/users")]
    public class UserController : ApiController
    {
        /// <summary>
        /// Change current user password
        /// </summary>
        [HttpPost("me/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var command = new ChangePasswordCommand(
                User.GetUserId(),
                request.CurrentPassword,
                request.NewPassword);

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new trainer account
        /// </summary>
        [HttpPost("trainers")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(CreateTrainerAccountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTrainerAccount(CreateTrainerAccountCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleCreatedResult(result, response => response);
        }

        /// <summary>
        /// Delete trainer account (Admin only)
        /// </summary>
        [HttpDelete("trainers/{userId:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteTrainerAccount(Guid userId)
        {
            var result = await Mediator.Send(new DeleteTrainerCommand(userId));
            return HandleResult(result);
        }

        /// <summary>
        /// Edit current user profile
        /// </summary>
        [HttpPut("edit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
        {
            var command = new EditUserCommand(
                User.GetUserId(),
                request.FirstName,
                request.LastName,
                request.PhoneNumber);

            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Get all users in the system (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(PagedResult<GetAllUsersResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await Mediator.Send(new GetAllUsersQuery(page, pageSize));
            return HandleResult(result);
        }

        /// <summary>
        /// Get current authenticated user profile
        /// </summary>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile()
        {
            var result = await Mediator.Send(new GetUserByIdQuery(User.GetUserId()));
            return HandleResult(result);
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await Mediator.Send(new GetUserByIdQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Add roles to a user (Admin only)
        /// </summary>
        [HttpPost("{userId:Guid}/roles")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddRolesToUser(Guid userId, [FromBody] AddUserRolesRequest request)
        {
            var command = new AddUserRolesCommand(userId, request.RoleIds);
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
