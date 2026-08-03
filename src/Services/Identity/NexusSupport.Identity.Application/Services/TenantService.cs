using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Application.Services;

public sealed class TenantService(ITenantRepository repository) : ITenantService
{
    public async Task<IReadOnlyList<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<TenantDto?> GetByEntraTenantIdAsync(string entraTenantId, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByEntraTenantIdAsync(entraTenantId, cancellationToken));

    public async Task<TenantDto> CreateAsync(TenantDto tenant, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(tenant), cancellationToken));

    public async Task UpdateAsync(TenantDto tenant, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(tenant), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static TenantDto Map(TenantModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        EntraTenantId = model.EntraTenantId,
        IsActive = model.IsActive,
        CreateAt = model.CreateAt,
        UpdateAt = model.UpdateAt
    };

    private static TenantModel Map(TenantDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        EntraTenantId = dto.EntraTenantId,
        IsActive = dto.IsActive,
        CreateAt = dto.CreateAt,
        UpdateAt = dto.UpdateAt
    };

    private static TenantDto? MapNullable(TenantModel? model) => model is null ? null : Map(model);
}
