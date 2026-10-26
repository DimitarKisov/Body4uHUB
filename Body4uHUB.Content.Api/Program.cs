using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Application.Extensions;
using Body4uHUB.Content.Infrastructure.Extensions;
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
        .AddExceptionHandler<CustomExceptionHandler>();

    var app = builder.Build();

    app.UseExceptionHandler(options => { });
    app.UseForwardedHeaders();

    var isLocalLikeEnvironment = app.Environment.IsLocalLike();

    if (app.IsSwaggerEnabled())
    {
        app.UseSwaggerBasicAuth();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    var shouldUseHttpsRedirection = configuration.IsHttpsRedirectionEnabled(isLocalLikeEnvironment);
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

    Log.Information("Starting Body4uHUB.Content.Api");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}
