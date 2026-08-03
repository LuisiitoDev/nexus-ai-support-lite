namespace NexusSupport.Identity.Application.Dtos;

/// <summary>
/// Validated Entra claims the Gateway supplies after successful token validation, used to resolve
/// or auto-provision the local user on first sign-in (ADR-002).
/// </summary>
public class ProvisionUserRequestDto
{
    public required string EntraTenantId { get; set; }
    public required string Issuer { get; set; }
    public required string ExternalSubjectId { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string DisplayName { get; set; }
}
