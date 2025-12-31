using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Infrastructure.Persistence;
using Body4uHUB.Services.Infrastructure.Repositories;
using Body4uHUB.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Body4uHUB.Services.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<ServicesDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

            services
                .AddScoped<IServiceOrderRepository, ServiceOrderRepository>()
                .AddScoped<IServiceOrderReadRepository, ServiceOrderReadRepository>()
                .AddScoped<ITrainerProfileRepository, TrainerProfileRepository>()
                .AddScoped<ITrainerProfileReadRepository, TrainerProfileReadRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
