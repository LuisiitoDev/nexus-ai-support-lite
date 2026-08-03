namespace NexusSupport.Identity.Api.Constants;

/// <summary>
/// Centralizes every HTTP route exposed by the Identity API so endpoint mapping and
/// documentation stay in sync with a single source of truth.
/// </summary>
public static class ApiRoutes
{
    private const string Base = "/api";

    public static class Users
    {
        public const string Group = Base + "/users";
        public const string ById = "/{id:guid}";
        public const string ByExternalSubject = "/external-subject";
        public const string Provision = "/provision";
    }

    public static class Tenants
    {
        public const string Group = Base + "/tenants";
        public const string ById = "/{id:guid}";
        public const string ByEntraTenantId = "/by-entra-tenant/{entraTenantId}";
    }

    public static class TenantMemberships
    {
        public const string Group = Base + "/tenant-memberships";
        public const string ById = "/{id:guid}";
        public const string ByUser = "/by-user/{userId:guid}";
        public const string ByTenantAndUser = "/by-tenant/{tenantId:guid}/by-user/{userId:guid}";
    }

    public static class Roles
    {
        public const string Group = Base + "/roles";
        public const string ById = "/{id:int}";
        public const string ByCode = "/by-code/{code}";
    }

    public static class MembershipRoles
    {
        public const string Group = Base + "/membership-roles";
        public const string ById = "/{id:int}";
        public const string ByTenantMembership = "/by-tenant-membership/{tenantMembershipId:guid}";
    }
}
