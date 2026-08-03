using NexusSupport.Identity.Application.Dtos;

namespace NexusSupport.Identity.Application.Interfaces;

public interface IRolService
{
    Task<IReadOnlyList<RolDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RolDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RolDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<RolDto> CreateAsync(RolDto rol, CancellationToken cancellationToken = default);
    Task UpdateAsync(RolDto rol, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
