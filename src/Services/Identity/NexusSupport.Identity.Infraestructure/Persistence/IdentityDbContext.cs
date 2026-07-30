using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infraestructure.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<TenantModel> Tenants => Set<TenantModel>();
    public DbSet<IdentityProviderModel> IdentityProviders => Set<IdentityProviderModel>();
    public DbSet<RolModel> Roles => Set<RolModel>();
    public DbSet<MembershipRoleModel> MembershipRoles => Set<MembershipRoleModel>();
    public DbSet<TenantMembershipModel> TenantMemberships => Set<TenantMembershipModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
