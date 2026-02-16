using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Body4uHUB.Shared.Api.HealthChecks
{
    public sealed class DatabaseReadinessHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        public DatabaseReadinessHealthCheck(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                return HealthCheckResult.Unhealthy("ConnectionStrings:DefaultConnection is missing.");
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                return HealthCheckResult.Healthy("SQL Server is reachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("SQL Server is not reachable.", ex);
            }
        }
    }
}
