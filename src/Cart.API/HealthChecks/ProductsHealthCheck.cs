using Microsoft.Extensions.Diagnostics.HealthChecks;
using Cart.API.Clients;

namespace Cart.API.HealthChecks;

public class ProductsHealthCheck : IHealthCheck
{
    private readonly ProductsClient _productsClient;

    public ProductsHealthCheck(ProductsClient productsClient)
    {
        _productsClient = productsClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        bool vivo = await _productsClient.EstaVivoAsync(cancellationToken);

        if (vivo)
        {
            return HealthCheckResult.Healthy("Products.API responde.");
        }

        return HealthCheckResult.Degraded("Products.API no responde.");
    }
}