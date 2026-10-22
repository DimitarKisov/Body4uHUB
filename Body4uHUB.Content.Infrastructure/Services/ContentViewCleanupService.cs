using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Content.Infrastructure.Services;

/// <summary>
/// Deletes view marks of previous days; only today's are needed to count a visitor once per day.
/// Runs hourly, so a restart or a missed run does not leave old rows for long.
/// </summary>
internal sealed class ContentViewCleanupService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<ContentViewCleanupService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<ContentViewCleanupService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval, _timeProvider);

        do
        {
            try
            {
                await DeletePreviousDaysAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to delete content views of previous days.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task DeletePreviousDaysAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ContentDbContext>();

        var today = ViewTracker.Today(_timeProvider);

        await dbContext.ContentViews
            .Where(x => x.ViewDate < today)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
