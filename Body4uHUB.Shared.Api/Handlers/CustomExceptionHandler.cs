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
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger;
        private readonly IWebHostEnvironment _env;

        public CustomExceptionHandler(
            ILogger<CustomExceptionHandler> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

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
                    problemDetails.Extensions.Add("errors", validationEx.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    }));
                    break;

                case NotFoundException notFoundEx:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = notFoundEx.Error;
                    break;

                case BaseDomainException domainEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = domainEx.Error;
                    break;

                case InvalidOperationException invalidOpEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = invalidOpEx.Message;
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