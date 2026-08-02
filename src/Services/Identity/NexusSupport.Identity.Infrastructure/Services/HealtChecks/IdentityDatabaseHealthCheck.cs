using Microsoft.Extensions.Diagnostics.HealthChecks;
using NexusSupport.Identity.Infrastructure.Persistence;

namespace NexusSupport.Identity.Infrastructure.Services.HealthCheck;

public sealed class IdentityDatabaseHealthCheck(IdentityDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect
            ? HealthCheckResult.Healthy("Identity database is reachable.")
            : HealthCheckResult.Unhealthy("Identity database is not reachable.");
    }
}
