using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Body4uHUB.Shared.Api.Extensions
{
    public static class HostingExtensions
    {
        public static IServiceCollection AddForwardedHeadersConfiguration(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            return services;
        }

        public static IServiceCollection AddHttpsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(1); //Later change to 60 days
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;

                var httpsPortValue = configuration["HTTPS_PORT"]
                    ?? configuration["ASPNETCORE_HTTPS_PORTS"]?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                if (int.TryParse(httpsPortValue, out var httpsPort))
                {
                    options.HttpsPort = httpsPort;
                }
            });

            return services;
        }

        public static bool IsHttpsRedirectionEnabled(this IConfiguration configuration, bool isLocalLikeEnvironment)
        {
            var configuredValue = configuration.GetValue<bool?>("HttpsRedirection:Enabled");
            if (configuredValue.HasValue)
            {
                return configuredValue.Value;
            }

            if (isLocalLikeEnvironment)
            {
                return true;
            }

            var hasHttpsPorts = !string.IsNullOrWhiteSpace(configuration["ASPNETCORE_HTTPS_PORTS"])
                || !string.IsNullOrWhiteSpace(configuration["HTTPS_PORT"]);

            var aspNetCoreUrls = configuration["ASPNETCORE_URLS"];
            var hasHttpsUrls = !string.IsNullOrWhiteSpace(aspNetCoreUrls)
                && aspNetCoreUrls
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase));

            return hasHttpsPorts || hasHttpsUrls;
        }
    }
}
