using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Application.Services;

public sealed class TenantMembershipService(ITenantMembershipRepository repository) : ITenantMembershipService
{
    public async Task<IReadOnlyList<TenantMembershipDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<TenantMembershipDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<TenantMembershipDto?> GetByTenantAndUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByTenantAndUserAsync(tenantId, userId, cancellationToken));

    public async Task<IReadOnlyList<TenantMembershipDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => (await repository.GetByUserIdAsync(userId, cancellationToken)).Select(Map).ToList();

    public async Task<TenantMembershipDto> CreateAsync(TenantMembershipDto tenantMembership, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(tenantMembership), cancellationToken));

    public async Task UpdateAsync(TenantMembershipDto tenantMembership, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(tenantMembership), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static TenantMembershipDto Map(TenantMembershipModel model) => new()
    {
        Id = model.Id,
        TenantId = model.TenantId,
        UserId = model.UserId,
        Status = model.Status,
        CreatedAt = model.CreatedAt,
        JoinAt = model.JoinAt,
        UpdateAt = model.UpdateAt
    };

    private static TenantMembershipModel Map(TenantMembershipDto dto) => new()
    {
        Id = dto.Id,
        TenantId = dto.TenantId,
        UserId = dto.UserId,
        Status = dto.Status,
        CreatedAt = dto.CreatedAt,
        JoinAt = dto.JoinAt,
        UpdateAt = dto.UpdateAt
    };

    private static TenantMembershipDto? MapNullable(TenantMembershipModel? model) => model is null ? null : Map(model);
}
