namespace NexusSupport.Identity.Domain.Models;

/// <summary>
/// Result of resolving or auto-provisioning a local user on a validated Entra sign-in (ADR-002).
/// </summary>
public enum ProvisioningOutcome
{
    /// <summary>First valid sign-in: a new user, membership, and default Requester role were created.</summary>
    Created,

    /// <summary>The user, its organization membership, and roles already existed.</summary>
    Existing,

    /// <summary>No organization is registered for the given Entra tenant id.</summary>
    TenantNotFound,

    /// <summary>The organization exists but is not enabled.</summary>
    TenantDisabled,

    /// <summary>The user exists but is locally disabled.</summary>
    UserDisabled,

    /// <summary>The user's membership in this organization exists but is disabled.</summary>
    MembershipDisabled
}
