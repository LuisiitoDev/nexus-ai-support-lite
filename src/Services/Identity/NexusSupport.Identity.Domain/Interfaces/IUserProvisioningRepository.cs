using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Domain.Interfaces
{
    /// <summary>
    /// Resolves or creates the local user on a validated Entra sign-in (ADR-002, first-login provisioning).
    /// Implementations must resolve the tenant, user, membership, and role assignment atomically.
    /// </summary>
    public interface IUserProvisioningRepository
    {
        Task<UserProvisioningResultModel> ProvisionAsync(
            string entraTenantId,
            string issuer,
            string externalSubjectId,
            string email,
            string firstName,
            string lastName,
            string displayName,
            CancellationToken cancellationToken = default);
    }
}
