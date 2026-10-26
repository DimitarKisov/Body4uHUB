using Body4uHUB.Shared.Api.Extensions;
using Body4uHUB.Shared.Api.HealthChecks;
using Body4uHUB.Shared.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Body4uHUB.Content.Api.Extensions;

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
        services.ConfigureSwagger(
            "Body4uHUB Content & Community API",
            "API за управление на статии, форуми, коментари и bookmarks");
        services.AddValidatedOptions<JwtSettings>(configuration, JwtSettings.SectionName);
        services.AddJwtAuthentication();
        services.AddAuthorizationPolicies();
        services.AddRateLimitPolicies();

        return services;
    }

    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["liveness"])
            .AddCheck<StartupHealthCheck>("startup", tags: ["startup"])
            .AddCheck<DatabaseReadinessHealthCheck>("sql-server", tags: ["readiness"]);

        return services;
    }

    private static IServiceCollection AddRateLimitPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPerUserFixedWindowPolicy(RateLimitPolicies.CreateComment, permitLimit: 10, TimeSpan.FromMinutes(1));
            options.AddPerUserFixedWindowPolicy(RateLimitPolicies.CreateForumPost, permitLimit: 10, TimeSpan.FromMinutes(1));
        });

        return services;
    }
}
