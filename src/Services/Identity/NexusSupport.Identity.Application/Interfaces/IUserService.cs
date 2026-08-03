using NexusSupport.Identity.Application.Dtos;

namespace NexusSupport.Identity.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByExternalSubjectAsync(string issuer, string externalSubjectId, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserDto user, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserDto user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
