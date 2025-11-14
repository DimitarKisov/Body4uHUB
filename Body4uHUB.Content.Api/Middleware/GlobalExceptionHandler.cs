namespace Body4uHUB.Content.Api.Middleware
{
    using Body4uHUB.Shared.Exceptions;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
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
                        message = notFoundEx.Message
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

                case UnauthorizedException unauthorizedEx:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        statusCode,
                        message = unauthorizedEx.Message
                    };
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = new
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