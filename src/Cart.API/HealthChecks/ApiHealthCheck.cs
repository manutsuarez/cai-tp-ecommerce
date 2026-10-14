using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cart.API.HealthChecks;

public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("El servicio está funcionando."));
    }
}