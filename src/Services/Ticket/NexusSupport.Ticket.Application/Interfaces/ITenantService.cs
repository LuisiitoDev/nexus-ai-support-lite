using NexusSupport.Ticket.Application.Dtos;

namespace NexusSupport.Ticket.Application.Interfaces
{
    public interface ITenantService
    {
        Task<IReadOnlyList<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TenantDto> CreateAsync(TenantDto tenant, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TenantDto tenant, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
