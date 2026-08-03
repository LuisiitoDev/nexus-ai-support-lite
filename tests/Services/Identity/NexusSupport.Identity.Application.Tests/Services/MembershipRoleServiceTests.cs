using Moq;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Services;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;
using Xunit;

namespace NexusSupport.Identity.Application.Tests.Services;

public class MembershipRoleServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedMembershipRoles()
    {
        var model = CreateModel();
        var repository = new Mock<IMembershipRoleRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new MembershipRoleService(repository.Object);

        var result = await service.GetAllAsync();

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedMembershipRole_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<IMembershipRoleRepository>();
        repository.Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>())).ReturnsAsync(model);
        var service = new MembershipRoleService(repository.Object);

        var dto = await service.GetByIdAsync(model.Id);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<IMembershipRoleRepository>();
        repository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MembershipRoleModel?)null);
        var service = new MembershipRoleService(repository.Object);

        var dto = await service.GetByIdAsync(999);

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetByTenantMembershipIdAsync_ReturnsMappedMembershipRoles()
    {
        var model = CreateModel();
        var repository = new Mock<IMembershipRoleRepository>();
        repository.Setup(r => r.GetByTenantMembershipIdAsync(model.TenantMembershipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([model]);
        var service = new MembershipRoleService(repository.Object);

        var result = await service.GetByTenantMembershipIdAsync(model.TenantMembershipId);

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task CreateAsync_PassesMappedModelToRepositoryAndReturnsMappedResult()
    {
        var dto = CreateDto();
        var repository = new Mock<IMembershipRoleRepository>();
        repository.Setup(r => r.CreateAsync(It.Is<MembershipRoleModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MembershipRoleModel m, CancellationToken _) => m);
        var service = new MembershipRoleService(repository.Object);

        var result = await service.CreateAsync(dto);

        AssertEqual(dto, result);
    }

    [Fact]
    public async Task UpdateAsync_PassesMappedModelToRepository()
    {
        var dto = CreateDto();
        var repository = new Mock<IMembershipRoleRepository>();
        repository.Setup(r => r.UpdateAsync(It.Is<MembershipRoleModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        var service = new MembershipRoleService(repository.Object);

        await service.UpdateAsync(dto);

        repository.Verify();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        const int id = 42;
        var repository = new Mock<IMembershipRoleRepository>();
        repository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        var service = new MembershipRoleService(repository.Object);

        await service.DeleteAsync(id);

        repository.Verify();
    }

    private static MembershipRoleModel CreateModel() => new()
    {
        Id = 1,
        TenantMembershipId = Guid.NewGuid(),
        RoleId = 10,
        CreateAt = new DateTime(2026, 1, 2)
    };

    private static MembershipRoleDto CreateDto() => new()
    {
        Id = 2,
        TenantMembershipId = Guid.NewGuid(),
        RoleId = 20,
        CreateAt = new DateTime(2026, 2, 2)
    };

    private static bool MatchesDto(MembershipRoleModel model, MembershipRoleDto dto) =>
        model.Id == dto.Id && model.TenantMembershipId == dto.TenantMembershipId &&
        model.RoleId == dto.RoleId && model.CreateAt == dto.CreateAt;

    private static void AssertEqual(MembershipRoleModel model, MembershipRoleDto dto)
    {
        Assert.Equal(model.Id, dto.Id);
        Assert.Equal(model.TenantMembershipId, dto.TenantMembershipId);
        Assert.Equal(model.RoleId, dto.RoleId);
        Assert.Equal(model.CreateAt, dto.CreateAt);
    }

    private static void AssertEqual(MembershipRoleDto expected, MembershipRoleDto actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.TenantMembershipId, actual.TenantMembershipId);
        Assert.Equal(expected.RoleId, actual.RoleId);
        Assert.Equal(expected.CreateAt, actual.CreateAt);
    }
}
