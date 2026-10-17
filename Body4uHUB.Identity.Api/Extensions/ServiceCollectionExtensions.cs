using Body4uHUB.Identity.Application.Settings;
using Body4uHUB.Shared.Api.Extensions;
using Body4uHUB.Shared.Api.HealthChecks;
using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
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
                options.AddPerIpFixedWindowPolicy(RateLimitPolicies.ResendEmailConfirmation, permitLimit: 3, TimeSpan.FromMinutes(15));

                // Per IP rather than per email: a per-email limit would let anyone lock a victim out of their account.
                options.AddPerIpFixedWindowPolicy(RateLimitPolicies.Login, permitLimit: 5, TimeSpan.FromMinutes(1));

                // Each registration sends an email, so this also caps how much mail one client can trigger.
                options.AddPerIpFixedWindowPolicy(RateLimitPolicies.Register, permitLimit: 5, TimeSpan.FromHours(1));
            });

            return services;
        }

        private static void AddPerIpFixedWindowPolicy(this RateLimiterOptions options, string policyName, int permitLimit, TimeSpan window)
        {
            options.AddPolicy(policyName, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = window,
                        QueueLimit = 0
                    }));
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
