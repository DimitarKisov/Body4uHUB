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
        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        /// <summary>
        /// Handles Result pattern with value and returns appropriate HTTP response based on error type
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="result">Result object</param>
        /// <returns>IActionResult with appropriate status code</returns>
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return result.ErrorType switch
            {
                ErrorType.ResourceNotFound => UnprocessableEntity(new { error = result.Error }),
                ErrorType.BusinessRule => UnprocessableEntity(new { error = result.Error }),
                ErrorType.Conflict => Conflict(new { error = result.Error }),
                ErrorType.Unauthorized => Unauthorized(new { error = result.Error }),
                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" })
            };
        }

        /// <summary>
        /// Handles Result pattern with Value Object (ArticleId, CommentId, etc.) and extracts primitive value
        /// Usage: HandleResult(result, id => new { articleId = id })
        /// </summary>
        /// <typeparam name="T">Value Object type (ArticleId, CommentId, etc.)</typeparam>
        /// <param name="result">Result object</param>
        /// <param name="responseFactory">Function to create response object from extracted value</param>
        /// <returns>IActionResult with appropriate status code</returns>
        protected IActionResult HandleResult<T>(Result<T> result, Func<object, object> responseFactory)
        {
            if (result.IsSuccess)
            {
                // Use reflection to get Value property from Value Object
                var valueProperty = typeof(T).GetProperty("Value");
                if (valueProperty != null)
                {
                    var primitiveValue = valueProperty.GetValue(result.Value);
                    return Ok(responseFactory(primitiveValue));
                }

                // Fallback to default handling
                return Ok(responseFactory(result.Value));
            }

            return result.ErrorType switch
            {
                ErrorType.ResourceNotFound => UnprocessableEntity(new { error = result.Error }),
                ErrorType.BusinessRule => UnprocessableEntity(new { error = result.Error }),
                ErrorType.Conflict => Conflict(new { error = result.Error }),
                ErrorType.Unauthorized => Unauthorized(new { error = result.Error }),
                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" })
            };
        }

        /// <summary>
        /// Handles Result pattern without return value and returns appropriate HTTP response
        /// </summary>
        /// <param name="result">Result object</param>
        /// <returns>IActionResult with appropriate status code</returns>
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return result.ErrorType switch
            {
                ErrorType.ResourceNotFound => UnprocessableEntity(new { error = result.Error }),
                ErrorType.BusinessRule => UnprocessableEntity(new { error = result.Error }),
                ErrorType.Conflict => Conflict(new { error = result.Error }),
                ErrorType.Unauthorized => Unauthorized(new { error = result.Error }),
                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" })
            };
        }

        /// <summary>
        /// Handles Result pattern with CreatedAtAction response for POST operations
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="result">Result object</param>
        /// <param name="actionName">Action name for location header</param>
        /// <param name="routeValues">Route values for location header</param>
        /// <returns>IActionResult with appropriate status code</returns>
        protected IActionResult HandleCreatedResult<T>(Result<T> result, string actionName, object routeValues = null)
        {
            if (result.IsSuccess)
            {
                return CreatedAtAction(actionName, routeValues, result.Value);
            }

            return result.ErrorType switch
            {
                ErrorType.ResourceNotFound => UnprocessableEntity(new { error = result.Error }),
                ErrorType.BusinessRule => UnprocessableEntity(new { error = result.Error }),
                ErrorType.Conflict => Conflict(new { error = result.Error }),
                ErrorType.Unauthorized => Unauthorized(new { error = result.Error }),
                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" })
            };
        }
    }
}