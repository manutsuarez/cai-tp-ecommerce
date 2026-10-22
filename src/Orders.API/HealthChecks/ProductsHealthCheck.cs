using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.API.Clients;

namespace Orders.API.HealthChecks;

public class ProductsHealthCheck : IHealthCheck
{
    private readonly ProductsClient _client;

    public ProductsHealthCheck(ProductsClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        bool vivo = await _client.EstaVivoAsync(cancellationToken);

        if (vivo)
        {
            return HealthCheckResult.Healthy("Products.API responde.");
        }

        return HealthCheckResult.Degraded("Products.API no responde.");
    }
}