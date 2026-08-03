using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;
using NexusSupport.Identity.Domain.Interfaces;

namespace NexusSupport.Identity.Application.Services;

public sealed class UserProvisioningService(IUserProvisioningRepository repository) : IUserProvisioningService
{
    public async Task<ProvisionUserResultDto> ProvisionAsync(ProvisionUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await repository.ProvisionAsync(
            request.EntraTenantId,
            request.Issuer,
            request.ExternalSubjectId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.DisplayName,
            cancellationToken);

        return new ProvisionUserResultDto
        {
            Outcome = result.Outcome,
            UserId = result.UserId,
            TenantId = result.TenantId,
            TenantMembershipId = result.TenantMembershipId,
            Email = result.Email,
            DisplayName = result.DisplayName,
            Roles = result.RoleCodes
        };
    }
}
