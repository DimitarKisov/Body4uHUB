using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Identity.Infrastructure.Repositories;
using Body4uHUB.Identity.Infrastructure.Services;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Infrastructure.Interfaces;
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
                .AddScoped<IJwtTokenService, JwtTokenService>()
                .AddScoped<IPasswordHasherService, PasswordHasherService>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IDbInitializer, DbInitializer>();

            return services;
        }
    }
}
