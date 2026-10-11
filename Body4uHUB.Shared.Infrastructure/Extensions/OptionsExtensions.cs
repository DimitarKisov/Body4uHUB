using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Body4uHUB.Shared.Infrastructure.Extensions
{
    public static class OptionsExtensions
    {
        /// <summary>
        /// Binds a settings section and validates its data annotations when the host starts.
        /// Validation is not done here, so design-time tools (dotnet ef) that only build the host
        /// do not need the real values.
        /// </summary>
        public static IServiceCollection AddValidatedOptions<TSettings>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName)
            where TSettings : class
        {
            services
                .AddOptions<TSettings>()
                .Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
