using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.API.Clients;

namespace Orders.API.HealthChecks;

public class UsersHealthCheck : IHealthCheck
{
    private readonly UsersClient _client;

    public UsersHealthCheck(UsersClient client)
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
            return HealthCheckResult.Healthy("Users.API responde.");
        }

        return HealthCheckResult.Degraded("Users.API no responde.");
    }
}