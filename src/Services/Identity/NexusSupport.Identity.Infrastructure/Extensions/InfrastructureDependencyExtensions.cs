using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusSupport.Identity.Infrastructure.Persistence;

namespace NexusSupport.Identity.Infrastructure.Extensions;

public static class InfrastructureDependencyExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("NexusIdentity")));

        return services;
    }
}