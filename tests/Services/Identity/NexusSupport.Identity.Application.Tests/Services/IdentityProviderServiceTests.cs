using Moq;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Services;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;
using Xunit;

namespace NexusSupport.Identity.Application.Tests.Services;

public class IdentityProviderServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedIdentityProviders()
    {
        var model = CreateModel();
        var repository = new Mock<IIdentityProviderRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new IdentityProviderService(repository.Object);

        var result = await service.GetAllAsync();

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedIdentityProvider_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<IIdentityProviderRepository>();
        repository.Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>())).ReturnsAsync(model);
        var service = new IdentityProviderService(repository.Object);

        var dto = await service.GetByIdAsync(model.Id);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<IIdentityProviderRepository>();
        repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityProviderModel?)null);
        var service = new IdentityProviderService(repository.Object);

        var dto = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetByTenantIdAsync_ReturnsMappedIdentityProviders()
    {
        var model = CreateModel();
        var repository = new Mock<IIdentityProviderRepository>();
        repository.Setup(r => r.GetByTenantIdAsync(model.TenantId, It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new IdentityProviderService(repository.Object);

        var result = await service.GetByTenantIdAsync(model.TenantId);

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task CreateAsync_PassesMappedModelToRepositoryAndReturnsMappedResult()
    {
        var dto = CreateDto();
        var repository = new Mock<IIdentityProviderRepository>();
        repository.Setup(r => r.CreateAsync(It.Is<IdentityProviderModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityProviderModel m, CancellationToken _) => m);
        var service = new IdentityProviderService(repository.Object);

        var result = await service.CreateAsync(dto);

        AssertEqual(dto, result);
    }

    [Fact]
    public async Task UpdateAsync_PassesMappedModelToRepository()
    {
        var dto = CreateDto();
        var repository = new Mock<IIdentityProviderRepository>();
        repository.Setup(r => r.UpdateAsync(It.Is<IdentityProviderModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        var service = new IdentityProviderService(repository.Object);

        await service.UpdateAsync(dto);

        repository.Verify();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IIdentityProviderRepository>();
        repository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        var service = new IdentityProviderService(repository.Object);

        await service.DeleteAsync(id);

        repository.Verify();
    }

    private static IdentityProviderModel CreateModel() => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        ProviderType = 1,
        ClientId = "client-1",
        ClientSecret = "secret-1",
        CallbackPath = "/signin-oidc-1",
        IsEnabled = true,
        CreateAt = new DateTime(2026, 1, 2),
        UpdateAt = new DateTime(2026, 1, 3)
    };

    private static IdentityProviderDto CreateDto() => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        ProviderType = 2,
        ClientId = "client-2",
        ClientSecret = "secret-2",
        CallbackPath = "/signin-oidc-2",
        IsEnabled = false,
        CreateAt = new DateTime(2026, 2, 2),
        UpdateAt = new DateTime(2026, 2, 3)
    };

    private static bool MatchesDto(IdentityProviderModel model, IdentityProviderDto dto) =>
        model.Id == dto.Id && model.TenantId == dto.TenantId && model.ProviderType == dto.ProviderType &&
        model.ClientId == dto.ClientId && model.ClientSecret == dto.ClientSecret &&
        model.CallbackPath == dto.CallbackPath && model.IsEnabled == dto.IsEnabled &&
        model.CreateAt == dto.CreateAt && model.UpdateAt == dto.UpdateAt;

    private static void AssertEqual(IdentityProviderModel model, IdentityProviderDto dto)
    {
        Assert.Equal(model.Id, dto.Id);
        Assert.Equal(model.TenantId, dto.TenantId);
        Assert.Equal(model.ProviderType, dto.ProviderType);
        Assert.Equal(model.ClientId, dto.ClientId);
        Assert.Equal(model.ClientSecret, dto.ClientSecret);
        Assert.Equal(model.CallbackPath, dto.CallbackPath);
        Assert.Equal(model.IsEnabled, dto.IsEnabled);
        Assert.Equal(model.CreateAt, dto.CreateAt);
        Assert.Equal(model.UpdateAt, dto.UpdateAt);
    }

    private static void AssertEqual(IdentityProviderDto expected, IdentityProviderDto actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.TenantId, actual.TenantId);
        Assert.Equal(expected.ProviderType, actual.ProviderType);
        Assert.Equal(expected.ClientId, actual.ClientId);
        Assert.Equal(expected.ClientSecret, actual.ClientSecret);
        Assert.Equal(expected.CallbackPath, actual.CallbackPath);
        Assert.Equal(expected.IsEnabled, actual.IsEnabled);
        Assert.Equal(expected.CreateAt, actual.CreateAt);
        Assert.Equal(expected.UpdateAt, actual.UpdateAt);
    }
}
