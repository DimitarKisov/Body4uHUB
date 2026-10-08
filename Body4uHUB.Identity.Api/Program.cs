using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Identity.Application.Extensions;
using Body4uHUB.Identity.Infrastructure.Extensions;
using Body4uHUB.Shared.Api.Extensions;
using Body4uHUB.Shared.Api.Handlers;
using Body4uHUB.Shared.Api.HealthChecks;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.ConfigureSerilog();

    var services = builder.Services;
    var configuration = builder.Configuration;

    services
        .AddApiServices(configuration, builder.Environment)
        .AddApplication()
        .AddInfrastructure(configuration)
        .AddSingleton<StartupHealthCheck>()
        .AddCustomHealthChecks()
        .AddProblemDetails()
        .AddExceptionHandler<CustomExceptionHandler>();

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseForwardedHeaders();
    app.UseStatusCodePages(async context =>
    {
        var response = context.HttpContext.Response;

        if (response.StatusCode == 404)
        {
            response.ContentType = "application/json";
            await response.WriteAsJsonAsync(new
            {
                error = "Търсеният ресурс не е намерен."
            });
        }
    });

    var isLocalLikeEnvironment = app.Environment.IsLocalLike();

    if (app.IsSwaggerEnabled())
    {
        app.UseSwaggerBasicAuth();
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
    app.UseRateLimiter();
    app.MapControllers();

    app.MapHealthChecks("/health/startup", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("startup")
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("liveness")
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("readiness")
    });

    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await dbInitializer.InitializeAsync();
    }

    var startupHealthCheck = app.Services.GetRequiredService<StartupHealthCheck>();
    startupHealthCheck.MarkStartupCompleted();

    Log.Information("Starting Body4uHUB.Identity.Api");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
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
