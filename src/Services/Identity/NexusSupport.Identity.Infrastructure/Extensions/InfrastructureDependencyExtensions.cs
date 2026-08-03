using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Infrastructure.Persistence;
using NexusSupport.Identity.Infrastructure.Persistence.Repositories;
using NexusSupport.Identity.Infrastructure.Services.HealthCheck;
namespace NexusSupport.Identity.Infrastructure.Extensions;

public static class InfrastructureDependencyExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NexusIdentity")));

        services.AddHealthChecks()
            .AddCheck<IdentityDatabaseHealthCheck>("identity-database");

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantMembershipRepository, TenantMembershipRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IMembershipRoleRepository, MembershipRoleRepository>();
        services.AddScoped<IUserProvisioningRepository, UserProvisioningRepository>();

        return services;
    }
}