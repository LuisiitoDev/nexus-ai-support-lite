using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Repositories;

public sealed class RolRepository(IdentityDbContext dbContext) : IRolRepository
{
    public async Task<IReadOnlyList<RolModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Roles.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<RolModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<RolModel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Code == code, cancellationToken);

    public async Task<RolModel> CreateAsync(RolModel rol, CancellationToken cancellationToken = default)
    {
        await dbContext.Roles.AddAsync(rol, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return rol;
    }

    public async Task UpdateAsync(RolModel rol, CancellationToken cancellationToken = default)
    {
        await dbContext.Roles
            .Where(r => r.Id == rol.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.Name, rol.Name)
                .SetProperty(r => r.Code, rol.Code)
                .SetProperty(r => r.Description, rol.Description)
                .SetProperty(r => r.IsActive, rol.IsActive), cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await dbContext.Roles.Where(r => r.Id == id).ExecuteDeleteAsync(cancellationToken);
}
