using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Infrastructure.Messaging;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Identity.Infrastructure.Repositories;
using Body4uHUB.Identity.Infrastructure.Services;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Body4uHUB.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

            services
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserReadRepository, UserReadRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IRoleReadRepository, RoleReadRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IDbInitializer, DbInitializer>()
                .AddScoped<IEventBus, EventBus>();

            services.AddEmailService();

            services
                .AddSingleton<IJwtTokenService, JwtTokenService>()
                .AddSingleton<IPasswordHasherService, PasswordHasherService>();

            services.AddMassTransitWithRabbitMq(configuration);

            return services;
        }

        private static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var massTransitSection = configuration.GetSection("MassTransit");

            var host = massTransitSection["Host"] ?? throw new InvalidOperationException("MassTransit:Host is required");
            var virtualHost = massTransitSection["VirtualHost"] ?? "/";
            var username = massTransitSection["Username"] ?? throw new InvalidOperationException("MassTransit:Username is required");
            var password = massTransitSection["Password"] ?? throw new InvalidOperationException("MassTransit:Password is required");

            var retryCount = int.Parse(massTransitSection["RetryCount"] ?? "5");
            var retryIntervalSeconds = int.Parse(massTransitSection["RetryIntervalSeconds"] ?? "5");
            var outboxQueryMessageLimit = int.Parse(massTransitSection["OutboxQueryMessageLimit"] ?? "100");
            var outboxQueryDelaySeconds = int.Parse(massTransitSection["OutboxQueryDelaySeconds"] ?? "1");

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, virtualHost, h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.UseMessageRetry(r => r.Interval(retryCount, TimeSpan.FromSeconds(retryIntervalSeconds)));
                });

                x.AddEntityFrameworkOutbox<IdentityDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();

                    // Duplicate detection window
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);

                    // Query delay - how often to check for new messages
                    o.QueryDelay = TimeSpan.FromSeconds(outboxQueryDelaySeconds);

                    // Query message limit - how many messages to process at once
                    o.QueryMessageLimit = outboxQueryMessageLimit;

                    o.DisableInboxCleanupService();
                });
            });

            return services;
        }
    }
}
