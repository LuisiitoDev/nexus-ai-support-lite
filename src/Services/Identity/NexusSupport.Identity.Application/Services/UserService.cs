using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Application.Services;

public sealed class UserService(IUserRepository repository) : IUserService
{
    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<UserDto?> GetByExternalSubjectAsync(string issuer, string externalSubjectId, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByExternalSubjectAsync(issuer, externalSubjectId, cancellationToken));

    public async Task<UserDto> CreateAsync(UserDto user, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(user), cancellationToken));

    public async Task UpdateAsync(UserDto user, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(user), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static UserDto Map(UserModel model) => new()
    {
        Id = model.Id,
        Issuer = model.Issuer,
        ExternalSubjectId = model.ExternalSubjectId,
        Email = model.Email,
        FirstName = model.FirstName,
        LastName = model.LastName,
        DisplayName = model.DisplayName,
        Status = model.Status,
        LastLogin = model.LastLogin,
        CreateAt = model.CreateAt,
        UpdateAt = model.UpdateAt
    };

    private static UserModel Map(UserDto dto) => new()
    {
        Id = dto.Id,
        Issuer = dto.Issuer,
        ExternalSubjectId = dto.ExternalSubjectId,
        Email = dto.Email,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        DisplayName = dto.DisplayName,
        Status = dto.Status,
        LastLogin = dto.LastLogin,
        CreateAt = dto.CreateAt,
        UpdateAt = dto.UpdateAt
    };

    private static UserDto? MapNullable(UserModel? model) => model is null ? null : Map(model);
}
