using Body4uHUB.Shared.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Body4uHUB.Shared.Api
{
    /// <summary>
    /// Base API controller with Result pattern handling
    /// All microservice controllers should inherit from this
    /// </summary>
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private ISender _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        /// <summary>
        /// Handles Result with value - returns 200 OK on success
        /// </summary>
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return MapError(result);
        }

        /// <summary>
        /// Handles Result with custom response transformation - returns 200 OK on success
        /// </summary>
        protected IActionResult HandleResult<T>(Result<T> result, Func<object, object> responseFactory)
        {
            if (result.IsSuccess)
            {
                return Ok(responseFactory(result.Value));
            }

            return MapError(result);
        }

        /// <summary>
        /// Handles Result without value - returns 204 NoContent on success
        /// </summary>
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return MapError(result);
        }

        /// <summary>
        /// Handles Result for POST operations with custom response transformation - returns 201 Created on success
        /// </summary>
        protected IActionResult HandleCreatedResult<T>(Result<T> result, Func<object, object> responseFactory)
        {
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created, responseFactory(result.Value));
            }

            return MapError(result);
        }

        /// <summary>
        /// Centralized error mapping - single source of truth
        /// </summary>
        private IActionResult MapError(Result result)
        {
            var problemDetails = new ProblemDetails
            {
                Detail = result.Error,
                Instance = HttpContext.Request.Path
            };

            return result.ErrorType switch
            {
                ErrorType.ResourceNotFound => CreateProblem(problemDetails,
                    StatusCodes.Status404NotFound, "Resource not found"),

                ErrorType.BusinessRule => CreateProblem(problemDetails,
                    StatusCodes.Status422UnprocessableEntity, "Business rule violation"),

                ErrorType.Conflict => CreateProblem(problemDetails,
                    StatusCodes.Status409Conflict, "Conflict"),

                ErrorType.Unauthorized => CreateProblem(problemDetails,
                    StatusCodes.Status401Unauthorized, "Unauthorized"),

                ErrorType.Forbidden => CreateProblem(problemDetails,
                    StatusCodes.Status403Forbidden, "Forbidden"),

                _ => CreateProblem(new ProblemDetails
                {
                    Detail = "Internal server error",
                    Instance = HttpContext.Request.Path
                },
                StatusCodes.Status500InternalServerError, "Internal server error")
            };
        }

        private ObjectResult CreateProblem(ProblemDetails problemDetails, int statusCode, string title)
        {
            problemDetails.Status = statusCode;
            problemDetails.Title = title;
            return new ObjectResult(problemDetails) { StatusCode = statusCode };
        }
    }
}