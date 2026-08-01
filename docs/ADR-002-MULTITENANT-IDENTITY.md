# ADR-002: Use Microsoft Entra ID Multitenant Authentication

**Status:** Accepted  
**Date:** August 1, 2026  
**Decision owners:** Nexus Support Lite team  
**Related documents:** `SYSTEM_CONTEXT.md`, `CONTAINER_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`

## Context

Nexus Support Lite is a multi-tenant SaaS platform. Each customer organization uses its own identity provider, while Nexus owns local organizations, users, roles, account status, and authorization.

Microsoft Entra ID is the only identity provider integrated in the MVP. The architecture must permit additional providers later without making them part of the initial implementation.

A Nexus Global Administrator must register and enable each customer organization before its users can access the platform. Nexus identifies the organization from the authenticated Entra tenant identifier and automatically provisions a valid first-time user as a **Requester**.

The authentication design must avoid maintaining a separate Nexus application registration and credential set for every customer while preserving an explicit tenant allowlist and strict organization isolation.

## Decision

Use **Microsoft Entra ID multitenant application registrations** owned by the Nexus provider tenant.

The MVP will use:

- One multitenant application registration for the browser-based frontend.
- One multitenant application registration representing the protected Nexus API.
- Organizational Microsoft accounts only; personal Microsoft accounts are not supported.
- An onboarding process that establishes the customer tenant's enterprise application and required consent before the organization is enabled in Nexus.
- The Entra tenant ID claim, `tid`, as the external organization identifier.
- Nexus-owned local records for organizations, identity-provider configuration, users, roles, and account status.
- Automatic local user provisioning on first valid sign-in with the **Requester** role.
- Synchronization of the user's name and email from validated Entra claims during sign-in.

A successful Entra authentication is necessary but not sufficient for access. Nexus must also confirm that:

1. The token is valid for the intended Nexus API.
2. The token comes from a supported Entra organizational tenant.
3. The `tid` maps to a registered and enabled Nexus organization.
4. The local user is enabled.
5. The requested action is allowed by the user's local roles and tenant context.

The frontend must use the OAuth 2.0 Authorization Code Flow with PKCE. Access tokens are intended for the API, must not be treated as application session data, and must not be stored in insecure browser persistence.

The API Gateway and each service that accepts access tokens must enforce token validation and authorization at its own trust boundary. Internal service calls must not rely solely on claims forwarded by the browser.

## Decision Drivers

- Support independent Microsoft Entra tenants for multiple customer organizations.
- Keep customer onboarding centralized under the Nexus provider.
- Avoid one application registration and credential lifecycle per customer.
- Separate authentication by Entra from authorization by Nexus.
- Reject unknown or disabled organizations even when authentication succeeds.
- Preserve local roles, account status, and domain authorization.
- Keep the design extensible for future identity providers.
- Prevent tenant identity from being supplied or overridden by an untrusted client.

## Tenant Resolution and Validation Rules

- The organization context is derived from the validated `tid` claim, never from a browser-supplied organization ID.
- The token signature, audience, issuer, lifetime, and required claims must be validated using Microsoft identity platform metadata and libraries.
- Issuer validation must account for multitenant endpoints while still binding the token issuer to the validated `tid`.
- Unknown, malformed, missing, or disabled tenant mappings must produce an access denial without revealing other tenant information.
- Every downstream data operation must be constrained to the resolved organization.
- A route value, request body, query parameter, or custom header must not override the authenticated tenant context.
- Local user and organization status must be checked after token validation and before access to business operations.
- Authorization decisions remain server-side and cannot depend only on frontend visibility.

## Organization Onboarding

1. A Nexus Global Administrator creates the organization record.
2. The administrator records the organization's Entra tenant ID and identity-provider configuration.
3. The customer completes the required Entra enterprise-application and consent process.
4. Nexus verifies the tenant configuration.
5. The Nexus Global Administrator enables the organization.
6. Users from that tenant may authenticate.
7. On first valid access, Nexus creates the local user as a Requester and associates it with the resolved organization.

The Nexus Global Administrator may manage organization and identity configuration but must not gain access to tenant operational data.

## Consequences

### Positive

- A single provider-owned SaaS identity configuration can serve multiple customer Entra tenants.
- Nexus avoids per-customer application registrations and credentials.
- Customer organizations retain authentication within their own Entra tenant.
- Tenant onboarding remains explicitly controlled by Nexus.
- Local roles and account status can evolve independently of Entra.
- Future identity providers can be added behind the Identity domain without changing Ticket or Notification domain ownership.

### Negative and trade-offs

- Multitenant issuer validation is more complex than single-tenant validation.
- Customer onboarding may require an administrator from the customer tenant to complete consent or enterprise-application configuration.
- A configuration error in tenant allowlisting could deny valid users or create an isolation risk.
- Automatic provisioning requires careful handling of stable external subject identifiers, claim changes, and duplicate email addresses.
- Two application registrations introduce separate redirect URI, permission, and lifecycle configuration.
- Supporting additional identity providers later will require provider-neutral identity linking rather than treating Entra-specific claims as the local user key.

## Alternatives Considered

### One application registration per customer

Rejected for the MVP because it creates repeated configuration, credentials, redirect management, consent handling, and operational drift for every organization.

### Single-tenant application registration

Rejected because it would authenticate only users from the Nexus provider tenant and would not satisfy the multi-company SaaS requirement.

### Email-first organization resolution

Rejected for the Entra MVP flow. Email domains are mutable, aliases may overlap, and an email supplied before authentication is not a trustworthy tenant boundary. The validated Entra tenant ID is the authoritative organization key.

### Use Entra roles or groups as the sole authorization model

Rejected because Nexus owns product roles, topic permissions, local account status, and tenant-specific authorization. External identity claims cannot replace those controls.

### Support multiple identity providers in the MVP

Rejected to limit initial scope. The architecture remains extensible, but only Microsoft Entra ID will be implemented initially.

## Security Requirements

- Use maintained Microsoft identity platform libraries instead of custom token parsing or signature validation.
- Request only the minimum delegated permissions required for sign-in and API access.
- Do not store customer credentials in source control or container images.
- Do not use email as the stable local identity key; persist the provider, tenant identifier, and immutable external subject identifier.
- Do not accept personal Microsoft accounts.
- Rotate and protect any application credentials used by confidential components.
- Record security-relevant onboarding, tenant enablement, role, and account-status changes.
- Return generic access-denied responses for unknown or disabled organizations.
- Test tenant isolation independently from role authorization.

## Reconsideration Triggers

Re-evaluate this decision if:

- A customer requires a dedicated application registration or isolated identity configuration.
- Microsoft Entra external identity capabilities become a better fit for the onboarding model.
- Additional identity providers become an approved product requirement.
- Regulatory requirements demand per-customer keys, credentials, or deployment isolation.
- The frontend architecture changes from a browser SPA to a server-side confidential client or Backend for Frontend.
- Customer-specific conditional-access or consent requirements cannot be supported reliably by the shared multitenant registration.

## Validation Criteria

Before production launch, tests must demonstrate that:

1. A user from a registered and enabled Entra tenant can authenticate.
2. A first-time valid user is provisioned locally as a Requester.
3. Name and email changes are synchronized without changing the local identity key.
4. A user from an unknown tenant is denied.
5. A user from a disabled organization is denied.
6. A locally disabled user is denied even when Entra authentication succeeds.
7. A token with an invalid signature, issuer, audience, lifetime, or tenant claim is denied.
8. A user cannot override the resolved tenant through request input.
9. Tokens issued for one tenant cannot retrieve or mutate another tenant's data.
10. Nexus roles and topic permissions are enforced independently from Entra authentication.
11. The Nexus Global Administrator can manage tenant identity configuration without reading tenant operational data.
12. The frontend and API registrations, redirect URIs, scopes, consent, and logout behavior work in at least two independent Entra test tenants.
