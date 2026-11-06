using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Products.API.HealthChecks;

/// <summary>
/// Chequeo de "listo": intenta abrir la base SQLite y hacer una consulta mínima.
/// </summary>
public class SqliteHealthCheck : IHealthCheck
{
    private readonly IConfiguration _config;

    public SqliteHealthCheck(IConfiguration config)
    {
        _config = config;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=products.db";

            using SqliteConnection conn = new SqliteConnection(connectionString);
            await conn.OpenAsync(cancellationToken);
            await conn.ExecuteScalarAsync<int>("SELECT 1");

            return HealthCheckResult.Healthy("La base SQLite responde.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("No se pudo conectar a la base SQLite.", ex);
        }
    }
}
