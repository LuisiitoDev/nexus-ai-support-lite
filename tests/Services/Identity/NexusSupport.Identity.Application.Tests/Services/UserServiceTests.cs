using Moq;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Services;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;
using Xunit;

namespace NexusSupport.Identity.Application.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedUsers()
    {
        var model = CreateModel();
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([model]);
        var service = new UserService(repository.Object);

        var result = await service.GetAllAsync();

        AssertEqual(model, Assert.Single(result));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedUser_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByIdAsync(model.Id, It.IsAny<CancellationToken>())).ReturnsAsync(model);
        var service = new UserService(repository.Object);

        var dto = await service.GetByIdAsync(model.Id);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((UserModel?)null);
        var service = new UserService(repository.Object);

        var dto = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetByExternalSubjectAsync_ReturnsMappedUser_WhenFound()
    {
        var model = CreateModel();
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByExternalSubjectAsync(model.Issuer, model.ExternalSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        var service = new UserService(repository.Object);

        var dto = await service.GetByExternalSubjectAsync(model.Issuer, model.ExternalSubjectId);

        Assert.NotNull(dto);
        AssertEqual(model, dto);
    }

    [Fact]
    public async Task GetByExternalSubjectAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByExternalSubjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);
        var service = new UserService(repository.Object);

        var dto = await service.GetByExternalSubjectAsync("issuer", "subject");

        Assert.Null(dto);
    }

    [Fact]
    public async Task CreateAsync_PassesMappedModelToRepositoryAndReturnsMappedResult()
    {
        var dto = CreateDto();
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.CreateAsync(It.Is<UserModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel m, CancellationToken _) => m);
        var service = new UserService(repository.Object);

        var result = await service.CreateAsync(dto);

        AssertEqual(dto, result);
    }

    [Fact]
    public async Task UpdateAsync_PassesMappedModelToRepository()
    {
        var dto = CreateDto();
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.UpdateAsync(It.Is<UserModel>(m => MatchesDto(m, dto)), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        var service = new UserService(repository.Object);

        await service.UpdateAsync(dto);

        repository.Verify();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        var service = new UserService(repository.Object);

        await service.DeleteAsync(id);

        repository.Verify();
    }

    private static UserModel CreateModel() => new()
    {
        Id = Guid.NewGuid(),
        Issuer = "https://issuer.example.com",
        ExternalSubjectId = "external-subject-1",
        Email = "user@example.com",
        FirstName = "Ada",
        LastName = "Lovelace",
        DisplayName = "Ada Lovelace",
        Status = 1,
        LastLogin = new DateTime(2026, 1, 1),
        CreateAt = new DateTime(2026, 1, 2),
        UpdateAt = new DateTime(2026, 1, 3)
    };

    private static UserDto CreateDto() => new()
    {
        Id = Guid.NewGuid(),
        Issuer = "https://issuer.example.com",
        ExternalSubjectId = "external-subject-2",
        Email = "other@example.com",
        FirstName = "Grace",
        LastName = "Hopper",
        DisplayName = "Grace Hopper",
        Status = 2,
        LastLogin = new DateTime(2026, 2, 1),
        CreateAt = new DateTime(2026, 2, 2),
        UpdateAt = new DateTime(2026, 2, 3)
    };

    private static bool MatchesDto(UserModel model, UserDto dto) =>
        model.Id == dto.Id && model.Issuer == dto.Issuer && model.ExternalSubjectId == dto.ExternalSubjectId &&
        model.Email == dto.Email && model.FirstName == dto.FirstName && model.LastName == dto.LastName &&
        model.DisplayName == dto.DisplayName && model.Status == dto.Status && model.LastLogin == dto.LastLogin &&
        model.CreateAt == dto.CreateAt && model.UpdateAt == dto.UpdateAt;

    private static void AssertEqual(UserModel model, UserDto dto)
    {
        Assert.Equal(model.Id, dto.Id);
        Assert.Equal(model.Issuer, dto.Issuer);
        Assert.Equal(model.ExternalSubjectId, dto.ExternalSubjectId);
        Assert.Equal(model.Email, dto.Email);
        Assert.Equal(model.FirstName, dto.FirstName);
        Assert.Equal(model.LastName, dto.LastName);
        Assert.Equal(model.DisplayName, dto.DisplayName);
        Assert.Equal(model.Status, dto.Status);
        Assert.Equal(model.LastLogin, dto.LastLogin);
        Assert.Equal(model.CreateAt, dto.CreateAt);
        Assert.Equal(model.UpdateAt, dto.UpdateAt);
    }

    private static void AssertEqual(UserDto expected, UserDto actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Issuer, actual.Issuer);
        Assert.Equal(expected.ExternalSubjectId, actual.ExternalSubjectId);
        Assert.Equal(expected.Email, actual.Email);
        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
        Assert.Equal(expected.DisplayName, actual.DisplayName);
        Assert.Equal(expected.Status, actual.Status);
        Assert.Equal(expected.LastLogin, actual.LastLogin);
        Assert.Equal(expected.CreateAt, actual.CreateAt);
        Assert.Equal(expected.UpdateAt, actual.UpdateAt);
    }
}
