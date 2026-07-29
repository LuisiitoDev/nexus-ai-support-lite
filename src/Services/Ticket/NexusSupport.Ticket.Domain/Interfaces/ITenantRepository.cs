using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Domain.Interfaces
{
    public interface ITenantRepository
    {
        Task<IReadOnlyList<TenantModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TenantModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TenantModel> CreateAsync(TenantModel tenant, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TenantModel tenant, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
