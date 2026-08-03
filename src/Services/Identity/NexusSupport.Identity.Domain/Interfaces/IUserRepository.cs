using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<UserModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<UserModel?> GetByExternalSubjectAsync(string issuer, string externalSubjectId, CancellationToken cancellationToken = default);
        Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserModel user, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
