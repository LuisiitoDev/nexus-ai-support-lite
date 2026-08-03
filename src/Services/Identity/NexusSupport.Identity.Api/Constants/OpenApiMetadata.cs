namespace NexusSupport.Identity.Api.Constants;

/// <summary>
/// Document-level, tag-level and operation-level OpenAPI metadata shared across endpoint definitions.
/// </summary>
public static class OpenApiMetadata
{
    public static class Document
    {
        public const string Name = "v1";
        public const string Title = "Nexus Identity API";
        public const string Route = "/docs";
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

    public static class Users
    {
        public const string GetAllName = "GetUsers";
        public const string GetAllSummary = "Lists users";
        public const string GetAllDescription = "Returns every locally provisioned user across all tenants.";

        public const string GetByIdName = "GetUserById";
        public const string GetByIdSummary = "Gets a user by id";
        public const string GetByIdDescription = "Returns a single user by its Nexus-local identifier.";

        public const string GetByExternalSubjectName = "GetUserByExternalSubject";
        public const string GetByExternalSubjectSummary = "Gets a user by external subject";
        public const string GetByExternalSubjectDescription =
            "Resolves the local user provisioned for a validated Entra issuer and subject identifier.";

        public const string CreateName = "CreateUser";
        public const string CreateSummary = "Creates a user";
        public const string CreateDescription = "Provisions a new local user record.";

        public const string UpdateName = "UpdateUser";
        public const string UpdateSummary = "Updates a user";
        public const string UpdateDescription =
            "Updates an existing user's profile, status, or last login. The user identifier travels in the payload.";

        public const string DeleteName = "DeleteUser";
        public const string DeleteSummary = "Deletes a user";
        public const string DeleteDescription = "Permanently removes a local user record.";
    }

    public static class Tenants
    {
        public const string GetAllName = "GetTenants";
        public const string GetAllSummary = "Lists organizations";
        public const string GetAllDescription = "Returns every organization (tenant) registered in Nexus.";

        public const string GetByIdName = "GetTenantById";
        public const string GetByIdSummary = "Gets an organization by id";
        public const string GetByIdDescription = "Returns a single organization (tenant) by its Nexus-local identifier.";

        public const string CreateName = "CreateTenant";
        public const string CreateSummary = "Registers an organization";
        public const string CreateDescription =
            "Registers a new organization (tenant). The organization is disabled until a Nexus Global Administrator enables it.";

        public const string UpdateName = "UpdateTenant";
        public const string UpdateSummary = "Updates an organization";
        public const string UpdateDescription =
            "Updates an existing organization's name or enabled status. The organization identifier travels in the payload.";

        public const string DeleteName = "DeleteTenant";
        public const string DeleteSummary = "Deletes an organization";
        public const string DeleteDescription = "Permanently removes an organization (tenant) record.";
    }

    public static class TenantMemberships
    {
        public const string GetAllName = "GetTenantMemberships";
        public const string GetAllSummary = "Lists tenant memberships";
        public const string GetAllDescription = "Returns every membership linking a user to an organization.";

        public const string GetByIdName = "GetTenantMembershipById";
        public const string GetByIdSummary = "Gets a tenant membership by id";
        public const string GetByIdDescription = "Returns a single membership by its Nexus-local identifier.";

        public const string GetByUserName = "GetTenantMembershipsByUser";
        public const string GetByUserSummary = "Lists a user's tenant memberships";
        public const string GetByUserDescription = "Returns every organization a given user belongs to.";

        public const string GetByTenantAndUserName = "GetTenantMembershipByTenantAndUser";
        public const string GetByTenantAndUserSummary = "Gets a user's membership within an organization";
        public const string GetByTenantAndUserDescription =
            "Returns the membership linking the given user to the given organization, if one exists.";

        public const string CreateName = "CreateTenantMembership";
        public const string CreateSummary = "Creates a tenant membership";
        public const string CreateDescription = "Links a user to an organization, e.g. on first valid sign-in.";

        public const string UpdateName = "UpdateTenantMembership";
        public const string UpdateSummary = "Updates a tenant membership";
        public const string UpdateDescription =
            "Updates the status of an existing tenant membership. The membership identifier travels in the payload.";

        public const string DeleteName = "DeleteTenantMembership";
        public const string DeleteSummary = "Deletes a tenant membership";
        public const string DeleteDescription = "Permanently removes the link between a user and an organization.";
    }

    public static class IdentityProviders
    {
        public const string GetAllName = "GetIdentityProviders";
        public const string GetAllSummary = "Lists identity providers";
        public const string GetAllDescription = "Returns every configured identity provider across all organizations.";

        public const string GetByIdName = "GetIdentityProviderById";
        public const string GetByIdSummary = "Gets an identity provider by id";
        public const string GetByIdDescription =
            "Returns a single identity provider configuration by its Nexus-local identifier.";

        public const string GetByTenantName = "GetIdentityProvidersByTenant";
        public const string GetByTenantSummary = "Lists an organization's identity providers";
        public const string GetByTenantDescription =
            "Returns every identity provider configured for a given organization (tenant).";

        public const string CreateName = "CreateIdentityProvider";
        public const string CreateSummary = "Creates an identity provider";
        public const string CreateDescription = "Registers a new identity provider configuration for an organization.";

        public const string UpdateName = "UpdateIdentityProvider";
        public const string UpdateSummary = "Updates an identity provider";
        public const string UpdateDescription =
            "Updates an existing identity provider's configuration or enabled status. The provider identifier travels in the payload.";

        public const string DeleteName = "DeleteIdentityProvider";
        public const string DeleteSummary = "Deletes an identity provider";
        public const string DeleteDescription = "Permanently removes an identity provider configuration.";
    }

    public static class Roles
    {
        public const string GetAllName = "GetRoles";
        public const string GetAllSummary = "Lists product roles";
        public const string GetAllDescription =
            "Returns every Nexus-managed product role (e.g. Requester, Agent, Administrator).";

        public const string GetByIdName = "GetRoleById";
        public const string GetByIdSummary = "Gets a product role by id";
        public const string GetByIdDescription = "Returns a single product role by its Nexus-local identifier.";

        public const string GetByCodeName = "GetRoleByCode";
        public const string GetByCodeSummary = "Gets a product role by code";
        public const string GetByCodeDescription = "Returns a single product role by its unique code.";

        public const string CreateName = "CreateRole";
        public const string CreateSummary = "Creates a product role";
        public const string CreateDescription = "Creates a new Nexus-managed product role.";

        public const string UpdateName = "UpdateRole";
        public const string UpdateSummary = "Updates a product role";
        public const string UpdateDescription =
            "Updates an existing product role's name, description, or active status. The role identifier travels in the payload.";

        public const string DeleteName = "DeleteRole";
        public const string DeleteSummary = "Deletes a product role";
        public const string DeleteDescription = "Permanently removes a product role.";
    }

    public static class MembershipRoles
    {
        public const string GetAllName = "GetMembershipRoles";
        public const string GetAllSummary = "Lists membership role assignments";
        public const string GetAllDescription = "Returns every product role assigned to a tenant membership.";

        public const string GetByIdName = "GetMembershipRoleById";
        public const string GetByIdSummary = "Gets a membership role assignment by id";
        public const string GetByIdDescription =
            "Returns a single membership role assignment by its Nexus-local identifier.";

        public const string GetByTenantMembershipName = "GetMembershipRolesByTenantMembership";
        public const string GetByTenantMembershipSummary = "Lists the roles assigned to a tenant membership";
        public const string GetByTenantMembershipDescription =
            "Returns every product role assigned to a given tenant membership.";

        public const string CreateName = "CreateMembershipRole";
        public const string CreateSummary = "Assigns a role to a tenant membership";
        public const string CreateDescription =
            "Assigns a Nexus-managed product role to a tenant membership, e.g. the default Requester role granted on first sign-in.";

        public const string UpdateName = "UpdateMembershipRole";
        public const string UpdateSummary = "Updates a membership role assignment";
        public const string UpdateDescription =
            "Updates an existing membership role assignment. The assignment identifier travels in the payload.";

        public const string DeleteName = "DeleteMembershipRole";
        public const string DeleteSummary = "Removes a role from a tenant membership";
        public const string DeleteDescription =
            "Permanently removes a product role assignment from a tenant membership.";
    }
}
