using Microsoft.EntityFrameworkCore;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infraestructure.Persistence.Repositories;

public sealed class TenantRepository(NexusDbContext dbContext) : ITenantRepository
{
    public async Task<IReadOnlyList<TenantModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Set<TenantModel>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<TenantModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Set<TenantModel>().AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<TenantModel> CreateAsync(TenantModel tenant, CancellationToken cancellationToken = default)
    {
        if (tenant.Id == Guid.Empty)
        {
            tenant.Id = Guid.NewGuid();
        }

        if (tenant.CreateAt == default)
        {
            tenant.CreateAt = DateTime.Now;
        }

        await dbContext.Set<TenantModel>().AddAsync(tenant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }

    public async Task<bool> UpdateAsync(TenantModel tenant, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<TenantModel>().FirstOrDefaultAsync(t => t.Id == tenant.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Name = tenant.Name;
        existing.IsActive = tenant.IsActive;
        dbContext.Entry(existing).Property(t => t.CreateAt).CurrentValue = tenant.CreateAt == default ? existing.CreateAt : tenant.CreateAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<TenantModel>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Set<TenantModel>().Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
