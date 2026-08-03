using Moq;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Services;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;
using Xunit;

namespace NexusSupport.Identity.Application.Tests.Services;

public class TenantMembershipServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedMemberships()
    {
        var model = CreateModel();
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new TenantMembershipService(repository.Object);

        var result = await service.GetAllAsync();

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedMembership_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>())).ReturnsAsync(model);
        var service = new TenantMembershipService(repository.Object);

        var dto = await service.GetByIdAsync(model.Id);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantMembershipModel?)null);
        var service = new TenantMembershipService(repository.Object);

        var dto = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetByTenantAndUserAsync_ReturnsMappedMembership_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.GetByTenantAndUserAsync(model.TenantId, model.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        var service = new TenantMembershipService(repository.Object);

        var dto = await service.GetByTenantAndUserAsync(model.TenantId, model.UserId);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByTenantAndUserAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.GetByTenantAndUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantMembershipModel?)null);
        var service = new TenantMembershipService(repository.Object);

        var dto = await service.GetByTenantAndUserAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsMappedMemberships()
    {
        var model = CreateModel();
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.GetByUserIdAsync(model.UserId, It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new TenantMembershipService(repository.Object);

        var result = await service.GetByUserIdAsync(model.UserId);

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task CreateAsync_PassesMappedModelToRepositoryAndReturnsMappedResult()
    {
        var dto = CreateDto();
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.CreateAsync(It.Is<TenantMembershipModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantMembershipModel m, CancellationToken _) => m);
        var service = new TenantMembershipService(repository.Object);

        var result = await service.CreateAsync(dto);

        AssertEqual(dto, result);
    }

    [Fact]
    public async Task UpdateAsync_PassesMappedModelToRepository()
    {
        var dto = CreateDto();
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.UpdateAsync(It.Is<TenantMembershipModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        var service = new TenantMembershipService(repository.Object);

        await service.UpdateAsync(dto);

        repository.Verify();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<ITenantMembershipRepository>();
        repository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        var service = new TenantMembershipService(repository.Object);

        await service.DeleteAsync(id);

        repository.Verify();
    }

    private static TenantMembershipModel CreateModel() => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        Status = 1,
        CreatedAt = new DateTime(2026, 1, 1),
        JoinAt = new DateTime(2026, 1, 2),
        UpdateAt = new DateTime(2026, 1, 3)
    };

    private static TenantMembershipDto CreateDto() => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        Status = 2,
        CreatedAt = new DateTime(2026, 2, 1),
        JoinAt = new DateTime(2026, 2, 2),
        UpdateAt = new DateTime(2026, 2, 3)
    };

    private static bool MatchesDto(TenantMembershipModel model, TenantMembershipDto dto) =>
        model.Id == dto.Id && model.TenantId == dto.TenantId && model.UserId == dto.UserId &&
        model.Status == dto.Status && model.CreatedAt == dto.CreatedAt && model.JoinAt == dto.JoinAt &&
        model.UpdateAt == dto.UpdateAt;

    private static void AssertEqual(TenantMembershipModel model, TenantMembershipDto dto)
    {
        Assert.Equal(model.Id, dto.Id);
        Assert.Equal(model.TenantId, dto.TenantId);
        Assert.Equal(model.UserId, dto.UserId);
        Assert.Equal(model.Status, dto.Status);
        Assert.Equal(model.CreatedAt, dto.CreatedAt);
        Assert.Equal(model.JoinAt, dto.JoinAt);
        Assert.Equal(model.UpdateAt, dto.UpdateAt);
    }

    private static void AssertEqual(TenantMembershipDto expected, TenantMembershipDto actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.TenantId, actual.TenantId);
        Assert.Equal(expected.UserId, actual.UserId);
        Assert.Equal(expected.Status, actual.Status);
        Assert.Equal(expected.CreatedAt, actual.CreatedAt);
        Assert.Equal(expected.JoinAt, actual.JoinAt);
        Assert.Equal(expected.UpdateAt, actual.UpdateAt);
    }
}
