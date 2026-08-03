using NexusSupport.Identity.Application.Dtos;

namespace NexusSupport.Identity.Application.Interfaces;

public interface ITenantService
{
    Task<IReadOnlyList<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantDto> CreateAsync(TenantDto tenant, CancellationToken cancellationToken = default);
    Task UpdateAsync(TenantDto tenant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
