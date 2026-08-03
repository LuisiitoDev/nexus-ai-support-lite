using NexusSupport.Identity.Application.Dtos;

namespace NexusSupport.Identity.Application.Interfaces;

public interface IIdentityProviderService
{
    Task<IReadOnlyList<IdentityProviderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IdentityProviderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IdentityProviderDto>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IdentityProviderDto> CreateAsync(IdentityProviderDto identityProvider, CancellationToken cancellationToken = default);
    Task UpdateAsync(IdentityProviderDto identityProvider, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
