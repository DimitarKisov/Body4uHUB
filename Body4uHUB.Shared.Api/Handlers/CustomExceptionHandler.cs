using Body4uHUB.Shared.Domain.Exceptions;
using Body4uHUB.Shared.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Shared.Api.Handlers
{
    public sealed class CustomExceptionHandler(
        ILogger<CustomExceptionHandler> logger,
        IWebHostEnvironment env)
        : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger = logger;
        private readonly IWebHostEnvironment _env = env;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception,
                "An unhandled exception occurred. Request: {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);

            switch (exception)
            {
                case ValidationException validationEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Validation failed";
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    problemDetails.Extensions.Add("errors", validationEx.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    }));
                    break;

                case UnauthorizedException unauthorizedEx:
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Detail = unauthorizedEx.Message;
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                    break;

                case DomainNotFoundException domainNotFoundEx:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Resource not found";
                    problemDetails.Detail = domainNotFoundEx.Error;
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    break;

                case DomainAuthorizationException domainAuthorizationEx:
                    problemDetails.Status = StatusCodes.Status403Forbidden;
                    problemDetails.Title = "Forbidden";
                    problemDetails.Detail = domainAuthorizationEx.Error;
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                    break;

                case BaseDomainException domainEx:
                    problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                    problemDetails.Title = "Business rule violation";
                    problemDetails.Detail = domainEx.Error;
                    problemDetails.Type = "https://tools.ietf.org/html/rfc4918#section-11.2";
                    break;

                case InvalidOperationException invalidOpEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid operation";
                    problemDetails.Detail = invalidOpEx.Message;
                    break;

                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "An internal server error occurred.";
                    if (_env.IsDevelopment())
                    {
                        problemDetails.Detail = exception.Message;
                        problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
                    }
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}