using NexusSupport.Identity.Application.Dtos;

namespace NexusSupport.Identity.Application.Interfaces;

public interface ITenantMembershipService
{
    Task<IReadOnlyList<TenantMembershipDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantMembershipDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantMembershipDto?> GetByTenantAndUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantMembershipDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TenantMembershipDto> CreateAsync(TenantMembershipDto tenantMembership, CancellationToken cancellationToken = default);
    Task UpdateAsync(TenantMembershipDto tenantMembership, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
