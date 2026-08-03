using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Application.Dtos;

public class ProvisionUserResultDto
{
    public required ProvisioningOutcome Outcome { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public Guid TenantMembershipId { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}
