using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Application.Services;

public sealed class MembershipRoleService(IMembershipRoleRepository repository) : IMembershipRoleService
{
    public async Task<IReadOnlyList<MembershipRoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<MembershipRoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<IReadOnlyList<MembershipRoleDto>> GetByTenantMembershipIdAsync(Guid tenantMembershipId, CancellationToken cancellationToken = default)
        => (await repository.GetByTenantMembershipIdAsync(tenantMembershipId, cancellationToken)).Select(Map).ToList();

    public async Task<MembershipRoleDto> CreateAsync(MembershipRoleDto membershipRole, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(membershipRole), cancellationToken));

    public async Task UpdateAsync(MembershipRoleDto membershipRole, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(membershipRole), cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static MembershipRoleDto Map(MembershipRoleModel model) => new()
    {
        Id = model.Id,
        TenantMembershipId = model.TenantMembershipId,
        RoleId = model.RoleId,
        CreateAt = model.CreateAt
    };

    private static MembershipRoleModel Map(MembershipRoleDto dto) => new()
    {
        Id = dto.Id,
        TenantMembershipId = dto.TenantMembershipId,
        RoleId = dto.RoleId,
        CreateAt = dto.CreateAt
    };

    private static MembershipRoleDto? MapNullable(MembershipRoleModel? model) => model is null ? null : Map(model);
}
