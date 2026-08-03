using Microsoft.Extensions.DependencyInjection;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Application.Services;

namespace NexusSupport.Identity.Application.Extensions;

public static class ApplicationDependencyExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITenantMembershipService, TenantMembershipService>();
        services.AddScoped<IRolService, RolService>();
        services.AddScoped<IMembershipRoleService, MembershipRoleService>();
        services.AddScoped<IUserProvisioningService, UserProvisioningService>();

        return services;
    }
}
