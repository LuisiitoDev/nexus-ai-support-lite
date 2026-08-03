using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Repositories;

public sealed class MembershipRoleRepository(IdentityDbContext dbContext) : IMembershipRoleRepository
{
    public async Task<IReadOnlyList<MembershipRoleModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.MembershipRoles.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<MembershipRoleModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await dbContext.MembershipRoles.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyList<MembershipRoleModel>> GetByTenantMembershipIdAsync(Guid tenantMembershipId, CancellationToken cancellationToken = default)
        => await dbContext.MembershipRoles.AsNoTracking().Where(m => m.TenantMembershipId == tenantMembershipId).ToListAsync(cancellationToken);

    public async Task<MembershipRoleModel> CreateAsync(MembershipRoleModel membershipRole, CancellationToken cancellationToken = default)
    {
        await dbContext.MembershipRoles.AddAsync(membershipRole, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return membershipRole;
    }

    public async Task UpdateAsync(MembershipRoleModel membershipRole, CancellationToken cancellationToken = default)
    {
        await dbContext.MembershipRoles
            .Where(m => m.Id == membershipRole.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.TenantMembershipId, membershipRole.TenantMembershipId)
                .SetProperty(m => m.RoleId, membershipRole.RoleId), cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await dbContext.MembershipRoles.Where(m => m.Id == id).ExecuteDeleteAsync(cancellationToken);
}
