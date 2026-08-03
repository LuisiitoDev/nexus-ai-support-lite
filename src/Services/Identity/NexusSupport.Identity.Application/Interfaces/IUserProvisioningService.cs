using NexusSupport.Identity.Application.Dtos;

namespace NexusSupport.Identity.Application.Interfaces;

public interface IUserProvisioningService
{
    Task<ProvisionUserResultDto> ProvisionAsync(ProvisionUserRequestDto request, CancellationToken cancellationToken = default);
}
