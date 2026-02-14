using Body4uHUB.Services.Api.Extensions;
using Body4uHUB.Services.Api.Middleware;
using Body4uHUB.Services.Application.Extensions;
using Body4uHUB.Services.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
    .SetApplicationName("Body4uHUB");
#else
// THIS WILL BE FOR PRODUCTION
#endif

// Configure Serilog
builder.ConfigureSerilog();

var services = builder.Services;
var configuration = builder.Configuration;

// Add services
services
    .AddApiServices(configuration)
    .AddApplication(configuration)
    .AddInfrastructure(configuration)
    .AddHealthChecks();

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<GlobalExceptionHandler>();
app.UseForwardedHeaders();

var isLocalLikeEnvironment = app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("Local", StringComparison.OrdinalIgnoreCase);

if (isLocalLikeEnvironment)

{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var shouldUseHttpsRedirection = ResolveHttpsRedirectionEnabled(configuration, isLocalLikeEnvironment);
if (shouldUseHttpsRedirection)
{
    if (!isLocalLikeEnvironment)
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
}
else
{
    Log.Warning("HTTPS redirection is disabled. Configure HttpsRedirection:Enabled=true or set ASPNETCORE_HTTPS_PORTS/HTTPS_PORT/ASPNETCORE_URLS with an https endpoint.");
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
await dbInitializer.InitializeAsync();

try
{
    Log.Information("Starting Body4uHUB.Services.Api");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static bool ResolveHttpsRedirectionEnabled(IConfiguration configuration, bool isLocalLikeEnvironment)
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