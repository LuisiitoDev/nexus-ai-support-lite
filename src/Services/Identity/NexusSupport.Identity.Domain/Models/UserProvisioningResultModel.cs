namespace NexusSupport.Identity.Domain.Models;

public class UserProvisioningResultModel
{
    public required ProvisioningOutcome Outcome { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public Guid TenantMembershipId { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public IReadOnlyList<string> RoleCodes { get; set; } = [];
}
