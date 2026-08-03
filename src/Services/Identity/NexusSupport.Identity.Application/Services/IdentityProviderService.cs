using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Application.Services;

public sealed class IdentityProviderService(IIdentityProviderRepository repository) : IIdentityProviderService
{
    public async Task<IReadOnlyList<IdentityProviderDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<IdentityProviderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<IReadOnlyList<IdentityProviderDto>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => (await repository.GetByTenantIdAsync(tenantId, cancellationToken)).Select(Map).ToList();

    public async Task<IdentityProviderDto> CreateAsync(IdentityProviderDto identityProvider, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(identityProvider), cancellationToken));

    public async Task UpdateAsync(IdentityProviderDto identityProvider, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(identityProvider), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static IdentityProviderDto Map(IdentityProviderModel model) => new()
    {
        Id = model.Id,
        TenantId = model.TenantId,
        ProviderType = model.ProviderType,
        ClientId = model.ClientId,
        ClientSecret = model.ClientSecret,
        CallbackPath = model.CallbackPath,
        IsEnabled = model.IsEnabled,
        CreateAt = model.CreateAt,
        UpdateAt = model.UpdateAt
    };

    private static IdentityProviderModel Map(IdentityProviderDto dto) => new()
    {
        Id = dto.Id,
        TenantId = dto.TenantId,
        ProviderType = dto.ProviderType,
        ClientId = dto.ClientId,
        ClientSecret = dto.ClientSecret,
        CallbackPath = dto.CallbackPath,
        IsEnabled = dto.IsEnabled,
        CreateAt = dto.CreateAt,
        UpdateAt = dto.UpdateAt
    };

    private static IdentityProviderDto? MapNullable(IdentityProviderModel? model) => model is null ? null : Map(model);
}
