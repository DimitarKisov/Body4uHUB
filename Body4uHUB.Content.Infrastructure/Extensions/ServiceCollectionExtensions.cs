using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Content.Infrastructure.Repositories;
using Body4uHUB.Shared.Domain;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Body4uHUB.Content.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<ContentDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

            // Domain Repositories (Write)
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IArticleReadRepository, ArticleReadRepository>();
            services.AddScoped<IForumTopicRepository, ForumTopicRepository>();
            services.AddScoped<IBookmarkRepository, BookmarkRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}