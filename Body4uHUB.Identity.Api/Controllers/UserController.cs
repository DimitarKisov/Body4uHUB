using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Identity.Application.Commands.ChangePassword;
using Body4uHUB.Identity.Application.Commands.CreateTrainer;
using Body4uHUB.Identity.Application.Commands.EditUser;
using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Queries.GetAllUsers;
using Body4uHUB.Identity.Application.Queries.GetProfile;
using Body4uHUB.Shared.Api;
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
        [HttpPut("changePassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            command.UserId = User.GetUserId();
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new trainer account (Admin only)
        /// </summary>
        [HttpPost("createTrainer")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTrainerAccount(CreateTrainerAccountCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }

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

        /// <summary>
        /// Get all users in the system (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await Mediator.Send(new GetAllUsersQuery());
            return Ok(users);
        }

        /// <summary>
        /// Get current authenticated user profile
        /// </summary>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.GetUserId();
            var result = await Mediator.Send(new GetUserByIdQuery { Id = userId });
            return HandleResult(result);
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await Mediator.Send(new GetUserByIdQuery { Id = id });
            return HandleResult(result);
        }
    }
}
