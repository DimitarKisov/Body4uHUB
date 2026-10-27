using Body4uHUB.Identity.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Identity.Infrastructure.Services;

/// <summary>
/// Deletes sessions that have ended: every token of a family that has no active token left (lifetime or inactivity
/// over, logout, or revoked after a password or role change). Rotated tokens of a live session are kept, because
/// reuse detection needs them. Runs every 6 hours; deleting is idempotent, so several replicas can run it too.
/// </summary>
internal sealed class RefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<RefreshTokenCleanupService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<RefreshTokenCleanupService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval, _timeProvider);

        do
        {
            try
            {
                await DeleteEndedSessionsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to delete ended refresh token sessions.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task DeleteEndedSessionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var deleted = await dbContext.RefreshTokens
            .Where(x => !dbContext.RefreshTokens.Any(y => y.FamilyId == x.FamilyId && y.RevokedAt == null && y.ExpiresAt > now))
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted > 0)
        {
            _logger.LogInformation("Deleted {Count} refresh tokens of ended sessions.", deleted);
        }
    }
}
