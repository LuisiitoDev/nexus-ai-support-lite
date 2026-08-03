using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Domain.Interfaces
{
    public interface IIdentityProviderRepository
    {
        Task<IReadOnlyList<IdentityProviderModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IdentityProviderModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<IdentityProviderModel>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<IdentityProviderModel> CreateAsync(IdentityProviderModel identityProvider, CancellationToken cancellationToken = default);
        Task UpdateAsync(IdentityProviderModel identityProvider, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
