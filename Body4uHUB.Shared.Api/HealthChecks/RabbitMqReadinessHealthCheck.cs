using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Net.Sockets;

namespace Body4uHUB.Shared.Api.HealthChecks
{
    public sealed class RabbitMqReadinessHealthCheck(IOptions<MassTransitSettings> massTransitSettings) : IHealthCheck
    {
        private readonly MassTransitSettings _massTransitSettings = massTransitSettings.Value;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var host = _massTransitSettings.Host;
            var port = _massTransitSettings.Port;

            try
            {
                using var client = new TcpClient();
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(2));

                await client.ConnectAsync(host, port, timeoutCts.Token);
                return HealthCheckResult.Healthy("RabbitMQ is reachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"RabbitMQ is not reachable at {host}:{port}.", ex);
            }
        }
    }
}
