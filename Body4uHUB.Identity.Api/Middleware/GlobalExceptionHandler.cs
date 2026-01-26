using Body4uHUB.Shared.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Body4uHUB.Identity.Api.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandler(
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. Request: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            object response;

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        statusCode,
                        message = "Validation failed",
                        errors = validationEx.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            error = e.ErrorMessage
                        })
                    };
                    break;

                case NotFoundException notFoundEx:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        statusCode,
                        message = notFoundEx.Error
                    };
                    break;

                case BaseDomainException domainEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        statusCode,
                        message = domainEx.Error
                    };
                    break;

                case InvalidOperationException invalidOpEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        statusCode,
                        message = invalidOpEx.Message
                    };
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = _env.IsDevelopment()
                        ? new
                        {
                            statusCode,
                            message = exception.Message,
                            stackTrace = exception.StackTrace,
                            environment = _env.EnvironmentName
                        }
                        : new
                        {
                            statusCode,
                            message = "An internal server error occurred. Please try again later."
                        };
                    break;
            }

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}