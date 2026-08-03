using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Application.Services;

public sealed class RolService(IRolRepository repository) : IRolService
{
    public async Task<IReadOnlyList<RolDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<RolDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<RolDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByCodeAsync(code, cancellationToken));

    public async Task<RolDto> CreateAsync(RolDto rol, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(rol), cancellationToken));

    public async Task UpdateAsync(RolDto rol, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(rol), cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static RolDto Map(RolModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Code = model.Code,
        Description = model.Description,
        IsActive = model.IsActive,
        CreateAt = model.CreateAt
    };

    private static RolModel Map(RolDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Code = dto.Code,
        Description = dto.Description,
        IsActive = dto.IsActive,
        CreateAt = dto.CreateAt
    };

    private static RolDto? MapNullable(RolModel? model) => model is null ? null : Map(model);
}
