using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NexusSupport.Identity.Infrastructure.Extensions;
using NexusSupport.Identity.Infrastructure.Persistence;
using Xunit;

namespace NexusSupport.Identity.Infrastructure.Tests.Extensions;

public class InfrastructureDependencyExtensionsTests
{
    [Fact]
    public void AddInfrastructure_RegistersIdentityDbContext()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(BuildConfiguration());

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IdentityDbContext));
    }

    [Fact]
    public void AddInfrastructure_RegistersIdentityDatabaseHealthCheck()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(BuildConfiguration());

        using var provider = services.BuildServiceProvider();
        var healthCheckOptions = provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;

        Assert.Contains(healthCheckOptions.Registrations, registration => registration.Name == "identity-database");
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:NexusIdentity"] = "Server=fake;Database=fake;Trusted_Connection=True;"
            })
            .Build();
    }
}
