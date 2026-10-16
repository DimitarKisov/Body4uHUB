using Body4uHUB.Shared.Api.Extensions;
using Body4uHUB.Shared.Api.HealthChecks;
using Body4uHUB.Shared.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Body4uHUB.Services.Api.Extensions
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
            services.ConfigureSwagger(
                "Body4uHUB Services & Community API",
                "API за управление на статии, форуми, коментари и bookmarks");
            services.AddValidatedOptions<JwtSettings>(configuration, JwtSettings.SectionName);
            services.AddJwtAuthentication();
            services.AddAuthorizationPolicies();

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
