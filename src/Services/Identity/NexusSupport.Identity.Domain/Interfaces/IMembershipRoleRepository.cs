using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Domain.Interfaces
{
    public interface IMembershipRoleRepository
    {
        Task<IReadOnlyList<MembershipRoleModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<MembershipRoleModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MembershipRoleModel>> GetByTenantMembershipIdAsync(Guid tenantMembershipId, CancellationToken cancellationToken = default);
        Task<MembershipRoleModel> CreateAsync(MembershipRoleModel membershipRole, CancellationToken cancellationToken = default);
        Task UpdateAsync(MembershipRoleModel membershipRole, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
