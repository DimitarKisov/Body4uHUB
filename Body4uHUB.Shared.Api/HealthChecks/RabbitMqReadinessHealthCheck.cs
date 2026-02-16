using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Sockets;

namespace Body4uHUB.Shared.Api.HealthChecks
{
    public sealed class RabbitMqReadinessHealthCheck : IHealthCheck
    {
        private readonly string _host;
        private const int _port = 5672;

        public RabbitMqReadinessHealthCheck(IConfiguration configuration)
        {
            _host = configuration["MassTransit:Host"];
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_host))
            {
                return HealthCheckResult.Unhealthy("RabbitMQ host is missing. Configure MassTransit:Host");
            }

            try
            {
                using var client = new TcpClient();
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(2));

                await client.ConnectAsync(_host, _port, timeoutCts.Token);
                return HealthCheckResult.Healthy("RabbitMQ is reachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"RabbitMQ is not reachable at {_host}:{_port}.", ex);
            }
        }
    }
}
