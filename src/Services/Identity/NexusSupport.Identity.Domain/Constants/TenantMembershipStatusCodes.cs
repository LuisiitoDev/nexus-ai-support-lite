namespace NexusSupport.Identity.Domain.Constants;

/// <summary>Fixed ids for the status lookup table backing <see cref="Models.TenantMembershipModel.Status"/>.</summary>
public static class TenantMembershipStatusCodes
{
    public const short EnabledId = 1;
    public const string Enabled = "Enabled";

    public const short DisabledId = 2;
    public const string Disabled = "Disabled";
}
