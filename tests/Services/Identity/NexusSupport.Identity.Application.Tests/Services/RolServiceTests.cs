using Moq;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Services;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;
using Xunit;

namespace NexusSupport.Identity.Application.Tests.Services;

public class RolServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedRoles()
    {
        var model = CreateModel();
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new RolService(repository.Object);

        var result = await service.GetAllAsync();

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedRole_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>())).ReturnsAsync(model);
        var service = new RolService(repository.Object);

        var dto = await service.GetByIdAsync(model.Id);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((RolModel?)null);
        var service = new RolService(repository.Object);

        var dto = await service.GetByIdAsync(999);

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetByCodeAsync_ReturnsMappedRole_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.GetByCodeAsync(model.Code, It.IsAny<CancellationToken>())).ReturnsAsync(model);
        var service = new RolService(repository.Object);

        var dto = await service.GetByCodeAsync(model.Code);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByCodeAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((RolModel?)null);
        var service = new RolService(repository.Object);

        var dto = await service.GetByCodeAsync("unknown-code");

        Assert.Null(dto);
    }

    [Fact]
    public async Task CreateAsync_PassesMappedModelToRepositoryAndReturnsMappedResult()
    {
        var dto = CreateDto();
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.CreateAsync(It.Is<RolModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RolModel m, CancellationToken _) => m);
        var service = new RolService(repository.Object);

        var result = await service.CreateAsync(dto);

        AssertEqual(dto, result);
    }

    [Fact]
    public async Task UpdateAsync_PassesMappedModelToRepository()
    {
        var dto = CreateDto();
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.UpdateAsync(It.Is<RolModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        var service = new RolService(repository.Object);

        await service.UpdateAsync(dto);

        repository.Verify();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        const int id = 42;
        var repository = new Mock<IRolRepository>();
        repository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        var service = new RolService(repository.Object);

        await service.DeleteAsync(id);

        repository.Verify();
    }

    private static RolModel CreateModel() => new()
    {
        Id = 1,
        Name = "Administrator",
        Code = "admin",
        Description = "Full access role",
        IsActive = true,
        CreateAt = new DateTime(2026, 1, 2)
    };

    private static RolDto CreateDto() => new()
    {
        Id = 2,
        Name = "Support Agent",
        Code = "support-agent",
        Description = "Handles support tickets",
        IsActive = false,
        CreateAt = new DateTime(2026, 2, 2)
    };

    private static bool MatchesDto(RolModel model, RolDto dto) =>
        model.Id == dto.Id && model.Name == dto.Name && model.Code == dto.Code &&
        model.Description == dto.Description && model.IsActive == dto.IsActive && model.CreateAt == dto.CreateAt;

    private static void AssertEqual(RolModel model, RolDto dto)
    {
        Assert.Equal(model.Id, dto.Id);
        Assert.Equal(model.Name, dto.Name);
        Assert.Equal(model.Code, dto.Code);
        Assert.Equal(model.Description, dto.Description);
        Assert.Equal(model.IsActive, dto.IsActive);
        Assert.Equal(model.CreateAt, dto.CreateAt);
    }

    private static void AssertEqual(RolDto expected, RolDto actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Code, actual.Code);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.IsActive, actual.IsActive);
        Assert.Equal(expected.CreateAt, actual.CreateAt);
    }
}
