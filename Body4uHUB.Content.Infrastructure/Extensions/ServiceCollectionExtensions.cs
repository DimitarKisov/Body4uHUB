using Body4uHUB.Content.Application.Abstractions;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Content.Infrastructure.Repositories;
using Body4uHUB.Content.Infrastructure.Services;
using Body4uHUB.Content.Infrastructure.Settings;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Body4uHUB.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Body4uHUB.Content.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ModifiedAtInterceptor>();

        services
            .AddDbContext<ContentDbContext>((serviceProvider, options) =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName))
            .AddInterceptors(serviceProvider.GetRequiredService<ModifiedAtInterceptor>()));

        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IArticleReadRepository, ArticleReadRepository>();
        services.AddScoped<IForumRepository, ForumRepository>();
        services.AddScoped<IForumReadRepository, ForumReadRepository>();
        services.AddScoped<IBookmarkRepository, BookmarkRepository>();
        services.AddScoped<IBookmarkReadRepository, BookmarkReadRepository>();
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.TryAddSingleton(TimeProvider.System);
        services.AddValidatedOptions<ViewTrackingSettings>(configuration, ViewTrackingSettings.SectionName);
        services.AddScoped<IViewTracker, ViewTracker>();
        services.AddHostedService<ContentViewCleanupService>();

        return services;
    }
}
