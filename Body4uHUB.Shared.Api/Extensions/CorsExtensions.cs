using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Body4uHUB.Shared.Api.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                throw new InvalidOperationException("Cors:AllowedOrigins must be configured in appsettings.json");
            }

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    if (environment.IsLocalLike())
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();

                        return;
                    }

                    policy.WithOrigins(allowedOrigins)
                          .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
                          .WithHeaders("Authorization", "Content-Type", "Accept")
                          .AllowCredentials()
                          .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });
            });

            return services;
        }
    }
}
