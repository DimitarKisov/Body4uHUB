using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Infrastructure.Email;
using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Body4uHUB.Shared.Infrastructure.Extensions
{
    public static class EmailServiceExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatedOptions<EmailSettings>(configuration, EmailSettings.SectionName);

            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
