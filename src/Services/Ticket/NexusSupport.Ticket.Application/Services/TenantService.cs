using NexusSupport.Ticket.Application.Dtos;
using NexusSupport.Ticket.Application.Interfaces;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Application.Services;

public sealed class TenantService(ITenantRepository repository) : ITenantService
{
    public async Task<IReadOnlyList<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<TenantDto> CreateAsync(TenantDto tenant, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(tenant), cancellationToken));

    public async Task<bool> UpdateAsync(TenantDto tenant, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(tenant), cancellationToken);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static TenantDto Map(TenantModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        IsActive = model.IsActive,
        CrateAt = model.CreateAt
    };

    private static TenantModel Map(TenantDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        IsActive = dto.IsActive,
        CreateAt = dto.CrateAt
    };

    private static TenantDto? MapNullable(TenantModel? model) => model is null ? null : Map(model);
}
