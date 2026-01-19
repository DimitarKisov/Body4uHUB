using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Infrastructure.Messaging;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Identity.Infrastructure.Repositories;
using Body4uHUB.Identity.Infrastructure.Services;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain;
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

            services
                .AddSingleton<IJwtTokenService, JwtTokenService>()
                .AddSingleton<IPasswordHasherService, PasswordHasherService>();

            services.AddMassTransitWithRabbitMq(configuration);

            return services;
        }

        private static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration.GetSection("MassTransit")["Host"];
            var virtualHost = configuration.GetSection("MassTransit")["VirtualHost"];
            var username = configuration.GetSection("MassTransit")["Username"];
            var password = configuration.GetSection("MassTransit")["Password"];
            var retryCount = int.Parse(configuration.GetSection("MassTransit")["RetryCount"]);
            var retryIntervalSeconds = int.Parse(configuration.GetSection("MassTransit")["RetryIntervalSeconds"]);
            var outboxQueryMessageLimit = int.Parse(configuration.GetSection("MassTransit")["OutboxQueryMessageLimit"]);
            var outboxQueryDelaySeconds = int.Parse(configuration.GetSection("MassTransit")["OutboxQueryDelaySeconds"]);

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
