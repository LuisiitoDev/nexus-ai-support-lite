using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Repositories;

public sealed class IdentityProviderRepository(IdentityDbContext dbContext) : IIdentityProviderRepository
{
    public async Task<IReadOnlyList<IdentityProviderModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.IdentityProviders.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IdentityProviderModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.IdentityProviders.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<IdentityProviderModel>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => await dbContext.IdentityProviders.AsNoTracking().Where(i => i.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task<IdentityProviderModel> CreateAsync(IdentityProviderModel identityProvider, CancellationToken cancellationToken = default)
    {
        await dbContext.IdentityProviders.AddAsync(identityProvider, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return identityProvider;
    }

    public async Task UpdateAsync(IdentityProviderModel identityProvider, CancellationToken cancellationToken = default)
    {
        await dbContext.IdentityProviders
            .Where(i => i.Id == identityProvider.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(i => i.TenantId, identityProvider.TenantId)
                .SetProperty(i => i.ProviderType, identityProvider.ProviderType)
                .SetProperty(i => i.ClientId, identityProvider.ClientId)
                .SetProperty(i => i.ClientSecret, identityProvider.ClientSecret)
                .SetProperty(i => i.CallbackPath, identityProvider.CallbackPath)
                .SetProperty(i => i.IsEnabled, identityProvider.IsEnabled)
                .SetProperty(i => i.UpdateAt, DateTime.Now), cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.IdentityProviders.Where(i => i.Id == id).ExecuteDeleteAsync(cancellationToken);
}
