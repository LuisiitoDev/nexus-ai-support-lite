using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Repositories;

public sealed class TenantMembershipRepository(IdentityDbContext dbContext) : ITenantMembershipRepository
{
    public async Task<IReadOnlyList<TenantMembershipModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.TenantMemberships.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<TenantMembershipModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.TenantMemberships.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<TenantMembershipModel?> GetByTenantAndUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
        => await dbContext.TenantMemberships.AsNoTracking().FirstOrDefaultAsync(t => t.TenantId == tenantId && t.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<TenantMembershipModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await dbContext.TenantMemberships.AsNoTracking().Where(t => t.UserId == userId).ToListAsync(cancellationToken);

    public async Task<TenantMembershipModel> CreateAsync(TenantMembershipModel tenantMembership, CancellationToken cancellationToken = default)
    {
        await dbContext.TenantMemberships.AddAsync(tenantMembership, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tenantMembership;
    }

    public async Task UpdateAsync(TenantMembershipModel tenantMembership, CancellationToken cancellationToken = default)
    {
        await dbContext.TenantMemberships
            .Where(t => t.Id == tenantMembership.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.TenantId, tenantMembership.TenantId)
                .SetProperty(t => t.UserId, tenantMembership.UserId)
                .SetProperty(t => t.Status, tenantMembership.Status)
                .SetProperty(t => t.JoinAt, tenantMembership.JoinAt)
                .SetProperty(t => t.UpdateAt, DateTime.Now), cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.TenantMemberships.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
}
