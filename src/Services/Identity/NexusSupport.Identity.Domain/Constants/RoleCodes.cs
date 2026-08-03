namespace NexusSupport.Identity.Domain.Constants;

/// <summary>
/// The Nexus-managed product roles (ADR-002). Fixed ids so seed data and first-login
/// provisioning can reference them without a lookup by name.
/// </summary>
public static class RoleCodes
{
    public const int RequesterId = 1;
    public const string Requester = "Requester";

    public const int AgentId = 2;
    public const string Agent = "Agent";

    public const int AdministratorId = 3;
    public const string Administrator = "Administrator";
}
