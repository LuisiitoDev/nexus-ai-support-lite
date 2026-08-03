using Moq;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Services;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;
using Xunit;

namespace NexusSupport.Identity.Application.Tests.Services;

public class UserProvisioningServiceTests
{
    [Fact]
    public async Task ProvisionAsync_PassesRequestFieldsToRepository()
    {
        var request = CreateRequest();
        var repository = new Mock<IUserProvisioningRepository>();
        repository
            .Setup(r => r.ProvisionAsync(
                request.EntraTenantId, request.Issuer, request.ExternalSubjectId,
                request.Email, request.FirstName, request.LastName, request.DisplayName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProvisioningResultModel { Outcome = ProvisioningOutcome.Created })
            .Verifiable();
        var service = new UserProvisioningService(repository.Object);

        await service.ProvisionAsync(request);

        repository.Verify();
    }

    [Fact]
    public async Task ProvisionAsync_MapsCreatedResultToDto()
    {
        var request = CreateRequest();
        var model = new UserProvisioningResultModel
        {
            Outcome = ProvisioningOutcome.Created,
            UserId = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            TenantMembershipId = Guid.NewGuid(),
            Email = request.Email,
            DisplayName = request.DisplayName,
            RoleCodes = ["Requester"]
        };
        var repository = new Mock<IUserProvisioningRepository>();
        repository
            .Setup(r => r.ProvisionAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        var service = new UserProvisioningService(repository.Object);

        var dto = await service.ProvisionAsync(request);

        Assert.Equal(model.Outcome, dto.Outcome);
        Assert.Equal(model.UserId, dto.UserId);
        Assert.Equal(model.TenantId, dto.TenantId);
        Assert.Equal(model.TenantMembershipId, dto.TenantMembershipId);
        Assert.Equal(model.Email, dto.Email);
        Assert.Equal(model.DisplayName, dto.DisplayName);
        Assert.Equal(model.RoleCodes, dto.Roles);
    }

    [Theory]
    [InlineData(ProvisioningOutcome.TenantNotFound)]
    [InlineData(ProvisioningOutcome.TenantDisabled)]
    [InlineData(ProvisioningOutcome.UserDisabled)]
    [InlineData(ProvisioningOutcome.MembershipDisabled)]
    public async Task ProvisionAsync_PropagatesDenialOutcomes(ProvisioningOutcome outcome)
    {
        var repository = new Mock<IUserProvisioningRepository>();
        repository
            .Setup(r => r.ProvisionAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProvisioningResultModel { Outcome = outcome });
        var service = new UserProvisioningService(repository.Object);

        var dto = await service.ProvisionAsync(CreateRequest());

        Assert.Equal(outcome, dto.Outcome);
    }

    private static ProvisionUserRequestDto CreateRequest() => new()
    {
        EntraTenantId = "11111111-1111-1111-1111-111111111111",
        Issuer = "https://login.microsoftonline.com/11111111-1111-1111-1111-111111111111/v2.0",
        ExternalSubjectId = "external-subject-1",
        Email = "user@example.com",
        FirstName = "Ada",
        LastName = "Lovelace",
        DisplayName = "Ada Lovelace"
    };
}
