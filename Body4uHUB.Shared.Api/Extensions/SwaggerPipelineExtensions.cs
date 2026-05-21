using Body4uHUB.Shared.Api.Middleware;
using Body4uHUB.Shared.Api.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Body4uHUB.Shared.Api.Extensions
{
    public static class SwaggerPipelineExtensions
    {
        public static IServiceCollection AddSwaggerOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SwaggerOptions>(configuration.GetSection(SwaggerOptions.SectionName));

            return services;
        }

        public static bool IsSwaggerEnabled(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;

            return options.Enabled;
        }

        public static IApplicationBuilder UseSwaggerBasicAuth(this IApplicationBuilder app)
        {
            app.UseWhen(
                context => context.Request.Path.StartsWithSegments("/swagger"),
                branch => branch.UseMiddleware<SwaggerBasicAuthMiddleware>());

            return app;
        }
    }
}
