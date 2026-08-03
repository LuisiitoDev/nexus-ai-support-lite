using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Domain.Interfaces
{
    public interface IRolRepository
    {
        Task<IReadOnlyList<RolModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<RolModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<RolModel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<RolModel> CreateAsync(RolModel rol, CancellationToken cancellationToken = default);
        Task UpdateAsync(RolModel rol, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
