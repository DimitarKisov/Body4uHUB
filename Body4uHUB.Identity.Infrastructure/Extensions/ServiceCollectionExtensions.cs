using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Infrastructure.Messaging;
using Body4uHUB.Identity.Infrastructure.Messaging.Consumers;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Identity.Infrastructure.Repositories;
using Body4uHUB.Identity.Infrastructure.Services;
using Body4uHUB.Identity.Infrastructure.Settings;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Body4uHUB.Shared.Infrastructure.Persistence;
using Body4uHUB.Shared.Infrastructure.Settings;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Body4uHUB.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ModifiedAtInterceptor>();

            services
                .AddDbContext<IdentityDbContext>((serviceProvider, options) =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(serviceProvider.GetRequiredService<ModifiedAtInterceptor>()));

            services
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserReadRepository, UserReadRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IRoleReadRepository, RoleReadRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IDbInitializer, DbInitializer>()
                .AddScoped<IEventBus, EventBus>();

            services
                .AddValidatedOptions<SeedDataSettings>(configuration, SeedDataSettings.SectionName)
                .AddValidatedOptions<JwtSettings>(configuration, JwtSettings.SectionName)
                .AddValidatedOptions<PasswordHashingSettings>(configuration, PasswordHashingSettings.SectionName);

            services.AddEmailService(configuration);

            services
                .AddSingleton<IJwtTokenService, JwtTokenService>()
                .AddSingleton<IPasswordHasherService, PasswordHasherService>();

            services.AddMassTransitWithRabbitMq(configuration);

            return services;
        }

        private static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatedOptions<MassTransitSettings>(configuration, MassTransitSettings.SectionName);

            // The outbox is configured during registration, before options can be resolved, so this read is not
            // validated. The same section is validated by ValidateOnStart before the bus starts.
            var outboxSettings = configuration.GetSection(MassTransitSettings.SectionName).Get<MassTransitSettings>()
                ?? new MassTransitSettings();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendEmailConfirmationConsumer>();
                x.AddConsumer<SendPasswordResetConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var settings = context.GetRequiredService<IOptions<MassTransitSettings>>().Value;

                    cfg.Host(settings.Host, (ushort)settings.Port, settings.VirtualHost, h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });

                    cfg.UseMessageRetry(r => r.Interval(settings.RetryCount, TimeSpan.FromSeconds(settings.RetryIntervalSeconds)));

                    cfg.ConfigureEndpoints(context);
                });

                x.AddEntityFrameworkOutbox<IdentityDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();

                    // Duplicate detection window
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);

                    // Query delay - how often to check for new messages
                    o.QueryDelay = TimeSpan.FromSeconds(outboxSettings.OutboxQueryDelaySeconds);

                    // Query message limit - how many messages to process at once
                    o.QueryMessageLimit = outboxSettings.OutboxQueryMessageLimit;

                    o.DisableInboxCleanupService();
                });
            });

            return services;
        }
    }
}
