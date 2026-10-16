using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Body4uHUB.Shared.Api.Extensions
{
    public static class DataProtectionExtensions
    {
        // Keys are persisted only where DataProtection:KeysPath is set (the Docker volume); otherwise the default store is used
        public static IServiceCollection AddDataProtectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var dataProtectionBuilder = services
                .AddDataProtection()
                .SetApplicationName("Body4uHUB");

            var keysPath = configuration["DataProtection:KeysPath"];
            if (!string.IsNullOrWhiteSpace(keysPath))
            {
                dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(keysPath));
            }

            return services;
        }
    }
}
