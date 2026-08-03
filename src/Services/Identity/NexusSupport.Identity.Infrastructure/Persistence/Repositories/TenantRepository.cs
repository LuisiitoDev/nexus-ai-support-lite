using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(IdentityDbContext dbContext) : ITenantRepository
{
    public async Task<IReadOnlyList<TenantModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Tenants.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<TenantModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<TenantModel?> GetByEntraTenantIdAsync(string entraTenantId, CancellationToken cancellationToken = default)
        => await dbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.EntraTenantId == entraTenantId, cancellationToken);

    public async Task<TenantModel> CreateAsync(TenantModel tenant, CancellationToken cancellationToken = default)
    {
        await dbContext.Tenants.AddAsync(tenant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }

    public async Task UpdateAsync(TenantModel tenant, CancellationToken cancellationToken = default)
    {
        await dbContext.Tenants
            .Where(t => t.Id == tenant.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Name, tenant.Name)
                .SetProperty(t => t.EntraTenantId, tenant.EntraTenantId)
                .SetProperty(t => t.IsActive, tenant.IsActive)
                .SetProperty(t => t.UpdateAt, DateTime.Now), cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Tenants.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken);
}
