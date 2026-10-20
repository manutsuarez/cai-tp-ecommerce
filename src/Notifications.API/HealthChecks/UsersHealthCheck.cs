using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notifications.API.Clients;

namespace Notifications.API.HealthChecks;

public class UsersHealthCheck : IHealthCheck
{
    private readonly UsersClient _usersClient;

    public UsersHealthCheck(UsersClient usersClient)
    {
        _usersClient = usersClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        bool vivo = await _usersClient.EstaVivoAsync(cancellationToken);

        if (vivo)
        {
            return HealthCheckResult.Healthy("Users.API responde.");
        }

        return HealthCheckResult.Degraded("Users.API no responde.");
    }
}