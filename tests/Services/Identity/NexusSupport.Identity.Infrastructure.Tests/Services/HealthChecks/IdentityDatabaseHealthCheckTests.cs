using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NexusSupport.Identity.Infrastructure.Persistence;
using NexusSupport.Identity.Infrastructure.Services.HealthCheck;
using Xunit;

namespace NexusSupport.Identity.Infrastructure.Tests.Services.HealthChecks;

public class IdentityDatabaseHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthy_WhenDatabaseCanConnect()
    {
        var dbContext = CreateContextWithConnectResult(canConnect: true);
        var healthCheck = new IdentityDatabaseHealthCheck(dbContext);

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("Identity database is reachable.", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthy_WhenDatabaseCannotConnect()
    {
        var dbContext = CreateContextWithConnectResult(canConnect: false);
        var healthCheck = new IdentityDatabaseHealthCheck(dbContext);

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Identity database is not reachable.", result.Description);
    }

    private static IdentityDbContext CreateContextWithConnectResult(bool canConnect)
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer("Server=fake;Database=fake;Trusted_Connection=True;")
            .Options;

        var mockContext = new Mock<IdentityDbContext>(options);
        var mockFacade = new Mock<DatabaseFacade>(mockContext.Object);

        mockFacade
            .Setup(f => f.CanConnectAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(canConnect);

        mockContext.Setup(c => c.Database).Returns(mockFacade.Object);

        return mockContext.Object;
    }
}
