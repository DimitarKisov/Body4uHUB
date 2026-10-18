using Body4uHUB.Identity.Application.Commands.ConfirmEmail;
using Body4uHUB.Identity.Application.Commands.ForgotPassword;
using Body4uHUB.Identity.Application.Commands.Login;
using Body4uHUB.Identity.Application.Commands.Register;
using Body4uHUB.Identity.Application.Commands.ResendEmailConfirmation;
using Body4uHUB.Identity.Application.Commands.ResetPassword;
using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Shared.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Body4uHUB.Identity.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : ApiController
    {
        /// <summary>
        /// Register a new user. A confirmation email is sent; the user can log in after confirming it.
        /// </summary>
        [HttpPost("register")]
        [EnableRateLimiting(RateLimitPolicies.Register)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return HandleCreatedResult(result, response => response);
        }


        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [EnableRateLimiting(RateLimitPolicies.Login)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Confirm email address with the token from the confirmation email
        /// </summary>
        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Send a new email confirmation link. Always returns 204, whether or not the email is registered or already confirmed.
        /// </summary>
        [HttpPost("resend-confirmation")]
        [EnableRateLimiting(RateLimitPolicies.ResendEmailConfirmation)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Email a password reset link (valid 1 hour, single use). Always returns 204, whether or not the email is registered.
        /// </summary>
        [HttpPost("forgot-password")]
        [EnableRateLimiting(RateLimitPolicies.ForgotPassword)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Set a new password with the token from the reset email. 422 when the link is invalid, expired or already used.
        /// </summary>
        [HttpPost("reset-password")]
        [EnableRateLimiting(RateLimitPolicies.ResetPassword)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }
    }
}
