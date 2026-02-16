using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Body4uHUB.Shared.Api.HealthChecks
{
    public sealed class StartupHealthCheck : IHealthCheck
    {
        private volatile bool _startupCompleted;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_startupCompleted
                ? HealthCheckResult.Healthy("Startup completed successfully.")
                : HealthCheckResult.Unhealthy("Startup is still in progress."));
        }

        public void MarkStartupCompleted()
        {
            _startupCompleted = true;
        }
    }
}
