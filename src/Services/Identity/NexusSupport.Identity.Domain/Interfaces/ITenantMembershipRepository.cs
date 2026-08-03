using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Domain.Interfaces
{
    public interface ITenantMembershipRepository
    {
        Task<IReadOnlyList<TenantMembershipModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TenantMembershipModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TenantMembershipModel?> GetByTenantAndUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantMembershipModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<TenantMembershipModel> CreateAsync(TenantMembershipModel tenantMembership, CancellationToken cancellationToken = default);
        Task UpdateAsync(TenantMembershipModel tenantMembership, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
