using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Body4uHUB.Shared.Api.Extensions
{
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();

            return builder;
        }
    }
}
