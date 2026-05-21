using Body4uHUB.Identity.Application.Commands.Login;
using Body4uHUB.Identity.Application.Commands.Register;
using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Mvc;

namespace Body4uHUB.Identity.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : ApiController
    {
        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleCreatedResult(result, response => response);
        }


        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await Mediator.Send(command);
            return HandleResult(result);
        }
    }
}
