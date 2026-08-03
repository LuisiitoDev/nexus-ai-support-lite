using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public async Task<IReadOnlyList<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<UserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<UserModel?> GetByExternalSubjectAsync(string issuer, string externalSubjectId, CancellationToken cancellationToken = default)
        => await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Issuer == issuer && u.ExternalSubjectId == externalSubjectId, cancellationToken);

    public async Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task UpdateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.Issuer, user.Issuer)
                .SetProperty(u => u.ExternalSubjectId, user.ExternalSubjectId)
                .SetProperty(u => u.Email, user.Email)
                .SetProperty(u => u.FirstName, user.FirstName)
                .SetProperty(u => u.LastName, user.LastName)
                .SetProperty(u => u.DisplayName, user.DisplayName)
                .SetProperty(u => u.Status, user.Status)
                .SetProperty(u => u.LastLogin, user.LastLogin)
                .SetProperty(u => u.UpdateAt, DateTime.Now), cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Users.Where(u => u.Id == id).ExecuteDeleteAsync(cancellationToken);
}
