using Microsoft.Extensions.Diagnostics.HealthChecks;
using Products.API.Clients;

namespace Products.API.HealthChecks;

public class OrdersHealthCheck : IHealthCheck
{
    private readonly OrdersClient _ordersClient;

    public OrdersHealthCheck(OrdersClient ordersClient)
    {
        _ordersClient = ordersClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        bool vivo = await _ordersClient.EstaVivoAsync(cancellationToken);

        if (vivo)
        {
            return HealthCheckResult.Healthy("Orders.API responde.");
        }

        return HealthCheckResult.Degraded("Orders.API no responde.");
    }
}