using Body4uHUB.Identity.Application.Settings;
using Body4uHUB.Shared.Api.Extensions;
using Body4uHUB.Shared.Api.HealthChecks;
using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace Body4uHUB.Identity.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddDataProtectionConfiguration(configuration);
            services.AddCorsPolicy(configuration, environment);
            services.AddForwardedHeadersConfiguration();
            services.AddHttpsConfiguration(configuration);
            services.AddSwaggerOptions(configuration);
            services.ConfigureSwagger("Body4uHUB Identity API");
            services.AddJwtAuthentication();
            services.AddAuthorizationPolicies();
            services.AddAppSettings(configuration);
            services.AddRateLimitPolicies();

            return services;
        }

        private static IServiceCollection AddRateLimitPolicies(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Partitioned by client IP, so the limit does not depend on whether the email exists.
                // The limiter is in memory, so with several replicas each one counts separately.
                options.AddPolicy(RateLimitPolicies.ResendEmailConfirmation, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(15),
                            QueueLimit = 0
                        }));
            });

            return services;
        }

        private static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<AppSettings>()
                .Bind(configuration.GetSection(AppSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["liveness"])
                .AddCheck<StartupHealthCheck>("startup", tags: ["startup"])
                .AddCheck<DatabaseReadinessHealthCheck>("sql-server", tags: ["readiness"])
                .AddCheck<RabbitMqReadinessHealthCheck>("rabbitmq", tags: ["readiness"]);

            return services;
        }
    }
}
