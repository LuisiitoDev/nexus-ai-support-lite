using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Domain.Interfaces
{
    public interface ITenantRepository
    {
        Task<IReadOnlyList<TenantModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TenantModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TenantModel?> GetByEntraTenantIdAsync(string entraTenantId, CancellationToken cancellationToken = default);
        Task<TenantModel> CreateAsync(TenantModel tenant, CancellationToken cancellationToken = default);
        Task UpdateAsync(TenantModel tenant, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
