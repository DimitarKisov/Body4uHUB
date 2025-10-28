using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Identity.Api.Middleware;
using Body4uHUB.Identity.Application.Extensions;
using Body4uHUB.Identity.Infrastructure.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting Body4uHUB.Identity.Api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    var services = builder.Services;
    var configuration = builder.Configuration;

    services
        .AddApplication()
        .AddInfrastructure(configuration)
        .AddControllers();

    services
        .AddEndpointsApiExplorer()
        .ConfigureSwagger();

    var app = builder.Build();

    app.UseMiddleware<GlobalExceptionHandler>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

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
