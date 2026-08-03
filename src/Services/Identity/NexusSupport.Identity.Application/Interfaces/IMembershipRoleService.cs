using NexusSupport.Identity.Application.Dtos;

namespace NexusSupport.Identity.Application.Interfaces;

public interface IMembershipRoleService
{
    Task<IReadOnlyList<MembershipRoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MembershipRoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MembershipRoleDto>> GetByTenantMembershipIdAsync(Guid tenantMembershipId, CancellationToken cancellationToken = default);
    Task<MembershipRoleDto> CreateAsync(MembershipRoleDto membershipRole, CancellationToken cancellationToken = default);
    Task UpdateAsync(MembershipRoleDto membershipRole, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
