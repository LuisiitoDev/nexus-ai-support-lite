namespace NexusSupport.Identity.Api.Constants;

/// <summary>
/// Document-level and tag-level OpenAPI metadata shared across endpoint definitions.
/// </summary>
public static class OpenApiMetadata
{
    public static class Document
    {
        public const string Title = "Nexus Identity API";
        public const string Version = "v1";
        public const string Description =
            "Owns organizations (tenants), local users, account status, product roles, " +
            "and the association between locally provisioned users and their validated " +
            "Microsoft Entra ID tenant, as defined in ADR-002.";
    }

    public static class Tags
    {
        public const string Users = "Users";
        public const string Tenants = "Tenants";
        public const string TenantMemberships = "Tenant Memberships";
        public const string IdentityProviders = "Identity Providers";
        public const string Roles = "Roles";
        public const string MembershipRoles = "Membership Roles";
    }
}
