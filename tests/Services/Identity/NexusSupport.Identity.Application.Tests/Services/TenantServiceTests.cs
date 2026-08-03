using Moq;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Services;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;
using Xunit;

namespace NexusSupport.Identity.Application.Tests.Services;

public class TenantServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedTenants()
    {
        var model = CreateModel();
        var repository = new Mock<ITenantRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new TenantService(repository.Object);

        var result = await service.GetAllAsync();

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedTenant_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<ITenantRepository>();
        repository.Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>())).ReturnsAsync(model);
        var service = new TenantService(repository.Object);

        var dto = await service.GetByIdAsync(model.Id);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<ITenantRepository>();
        repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((TenantModel?)null);
        var service = new TenantService(repository.Object);

        var dto = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(dto);
    }

    [Fact]
    public async Task CreateAsync_PassesMappedModelToRepositoryAndReturnsMappedResult()
    {
        var dto = CreateDto();
        var repository = new Mock<ITenantRepository>();
        repository.Setup(r => r.CreateAsync(It.Is<TenantModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantModel m, CancellationToken _) => m);
        var service = new TenantService(repository.Object);

        var result = await service.CreateAsync(dto);

        AssertEqual(dto, result);
    }

    [Fact]
    public async Task UpdateAsync_PassesMappedModelToRepository()
    {
        var dto = CreateDto();
        var repository = new Mock<ITenantRepository>();
        repository.Setup(r => r.UpdateAsync(It.Is<TenantModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        var service = new TenantService(repository.Object);

        await service.UpdateAsync(dto);

        repository.Verify();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<ITenantRepository>();
        repository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        var service = new TenantService(repository.Object);

        await service.DeleteAsync(id);

        repository.Verify();
    }

    private static TenantModel CreateModel() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Acme Corp",
        IsActive = true,
        CreateAt = new DateTime(2026, 1, 2),
        UpdateAt = new DateTime(2026, 1, 3)
    };

    private static TenantDto CreateDto() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Globex Corp",
        IsActive = false,
        CreateAt = new DateTime(2026, 2, 2),
        UpdateAt = new DateTime(2026, 2, 3)
    };

    private static bool MatchesDto(TenantModel model, TenantDto dto) =>
        model.Id == dto.Id && model.Name == dto.Name && model.IsActive == dto.IsActive &&
        model.CreateAt == dto.CreateAt && model.UpdateAt == dto.UpdateAt;

    private static void AssertEqual(TenantModel model, TenantDto dto)
    {
        Assert.Equal(model.Id, dto.Id);
        Assert.Equal(model.Name, dto.Name);
        Assert.Equal(model.IsActive, dto.IsActive);
        Assert.Equal(model.CreateAt, dto.CreateAt);
        Assert.Equal(model.UpdateAt, dto.UpdateAt);
    }

    private static void AssertEqual(TenantDto expected, TenantDto actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.IsActive, actual.IsActive);
        Assert.Equal(expected.CreateAt, actual.CreateAt);
        Assert.Equal(expected.UpdateAt, actual.UpdateAt);
    }
}
