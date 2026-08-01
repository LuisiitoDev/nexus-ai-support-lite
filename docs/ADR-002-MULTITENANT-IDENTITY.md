# ADR-002: Use Microsoft Entra ID Multitenant Authentication

**Status:** Accepted  
**Date:** August 1, 2026  
**Decision owners:** Nexus Support Lite team  
**Related documents:** `SYSTEM_CONTEXT.md`, `CONTAINER_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`

## Context

Nexus Support Lite is a multi-tenant SaaS platform for organizations that use independent Microsoft Entra ID tenants. Microsoft Entra ID provides authentication, while Nexus owns the local organization registry, users, account status, and the product roles **Requester**, **Agent**, and **Administrator**.

A Nexus Global Administrator must register and enable each customer organization before its users can access the platform. A successful Entra authentication alone must not grant access.

The design must:

- Avoid maintaining a separate application registration for every customer.
- Resolve the organization from a trusted token claim.
- Keep the microservices inaccessible from the public network.
- Centralize token validation at the API Gateway.
- Allow each microservice to enforce its own functional authorization rules.
- Avoid consulting Identity on every request while applying role and account-status changes promptly.

Microsoft Entra ID is the only identity provider included in the MVP. Support for additional providers remains a possible future evolution.

## Decision

Use **two provider-owned Microsoft Entra ID multitenant application registrations**:

1. One application registration for the browser-based frontend.
2. One application registration representing the protected Nexus API.

Only organizational Microsoft accounts are accepted. Personal Microsoft accounts are not supported.

Each customer organization must grant the required administrative consent during onboarding. Nexus does not create a separate application registration per customer.

The browser frontend uses the OAuth 2.0 Authorization Code Flow with PKCE.

## Authentication Boundary

The **API Gateway is the only component that validates Microsoft Entra ID access tokens**.

The Gateway validates, at minimum:

- Signature.
- Audience.
- Issuer.
- Expiration and validity period.
- Required claims.
- The Entra tenant claim, `tid`.

The microservices do not repeat Entra token validation and are not publicly reachable. Azure networking and Azure Container Apps ingress configuration must ensure that business microservices accept traffic only through the internal application environment and the API Gateway.

The public client cannot call Identity, Tickets, or Notifications directly.

## Tenant Resolution

The organization is resolved exclusively from the validated `tid` claim.

Nexus must never accept a tenant identifier from a route, query parameter, request body, browser-provided header, or other client input as a replacement for the authenticated tenant context.

After validating the token, the Gateway must confirm that the `tid` maps to an organization that is both:

- Registered in Nexus.
- Enabled for access.

An unknown, missing, malformed, or disabled tenant mapping results in access denial, even if Entra authenticated the user successfully.

## Local User Provisioning and Roles

Nexus manages the roles **Requester**, **Agent**, and **Administrator** locally. Entra groups and application roles are not the authoritative source for Nexus authorization.

On a user's first valid sign-in, Nexus automatically creates the local user with:

- The organization resolved from `tid`.
- The immutable external subject identifier from the validated token.
- The user's name.
- The user's email.
- The default **Requester** role.
- An enabled local status, subject to the organization's onboarding rules.

Email is not used as the stable identity key. Later changes to name or email may be synchronized without changing the user's local identity.

## Identity Resolution and Cache

After token validation and tenant resolution, the Gateway queries the Identity microservice for the local user, account status, and Nexus roles.

The Gateway caches this authorization context for an initial duration of **five minutes** to avoid querying Identity on every request.

The cache must be invalidated immediately when:

- The user is enabled or disabled.
- A role is assigned or removed.
- The organization is disabled.
- Another security-relevant identity change makes the cached authorization context stale.

If immediate invalidation cannot be completed, access may remain governed by the cached context for no longer than its five-minute lifetime. Failures and invalidation delays must be observable.

## Trusted Internal Identity Propagation

The Gateway does not rely on identity headers received from the client.

Before forwarding a request, it must:

1. Remove any client-supplied headers that use reserved internal identity or authentication names.
2. Validate the Entra token.
3. Resolve the organization and local user context.
4. Add trusted internal headers containing the minimum identity information required by the destination service, including the user identifier, tenant identifier, and roles.
5. Add the destination microservice's internal authentication key.

Each microservice has a **different internal shared key**. The service validates its own key before trusting the internal identity headers.

The keys are:

- Stored in GitHub Actions Secrets.
- Injected into the appropriate runtime configuration during CI/CD deployment.
- Never committed to source control.
- Never embedded in container images.
- Rotatable independently.

A key for one service must not grant access to another service.

Internal header names, canonical value formats, maximum sizes, and signing or rotation procedures must be documented before implementation.

## Authorization

Authentication is centralized at the Gateway, but **functional authorization remains the responsibility of each microservice**.

Each microservice uses the trusted user, tenant, and role context supplied by the Gateway to enforce its own business rules. It must also constrain all data operations to the trusted tenant context.

The Gateway may enforce broad route-level policies, but these do not replace domain authorization in the destination service.

## Organization Onboarding

1. A Nexus Global Administrator creates the organization record.
2. The administrator records the organization's Entra tenant ID.
3. A customer tenant administrator grants the required administrative consent.
4. Nexus verifies the tenant and enterprise-application configuration.
5. The Nexus Global Administrator enables the organization.
6. Users from the enabled tenant may authenticate.
7. On first valid access, Nexus creates the local user as a Requester.

The Nexus Global Administrator may manage organization and identity configuration but must not gain access to customer operational data solely because of that platform role.

## Decision Drivers

- Support independent Entra tenants for multiple customer organizations.
- Minimize per-customer identity configuration.
- Make the validated `tid` the authoritative tenant boundary.
- Separate authentication by Entra from authorization by Nexus.
- Keep product roles independent from Entra groups and roles.
- Avoid duplicate token validation logic in every microservice.
- Prevent direct public access to microservices.
- Reduce synchronous calls to Identity with a short-lived cache.
- Limit the impact of a compromised internal service key.
- Preserve independent domain authorization.

## Consequences

### Positive

- A shared SaaS identity configuration supports multiple customer tenants.
- Nexus avoids a separate application registration and credential lifecycle per customer.
- Token validation logic is centralized in the Gateway.
- Microservices remain focused on domain authorization rather than Entra integration.
- Local roles and user status can evolve independently from Entra.
- Per-service internal keys reduce the impact of one compromised key.
- The five-minute cache reduces latency and Identity load.

### Negative and trade-offs

- The Gateway becomes a critical authentication and identity-propagation component.
- Network isolation is mandatory because microservices do not validate the original Entra token.
- Shared internal keys require secure injection, independent rotation, and careful operational handling.
- Trusted identity headers create a private protocol that must be versioned and tested.
- Immediate cache invalidation introduces coordination between Identity and the Gateway.
- A failed invalidation can delay application of role or status changes for up to five minutes.
- Multitenant issuer validation and customer consent are more complex than a single-tenant setup.
- Two application registrations require separate redirect URI, scope, permission, and lifecycle management.

## Alternatives Considered

### Validate the Entra token in the Gateway and every microservice

Rejected for the MVP because microservices are private and must only be reachable through the Gateway. Repeating the same Entra validation in every service adds implementation and maintenance overhead. This decision depends on enforcing network isolation and internal request authentication.

### Forward the original Entra token to microservices

Rejected because the selected design centralizes Entra authentication at the Gateway. Services receive only the minimum trusted identity context required for authorization.

### Trust internal headers based only on network isolation

Rejected because network isolation alone does not authenticate the caller. Each service also requires its own internal shared key.

### Use one internal key for all microservices

Rejected because compromise of one shared key would permit impersonation across every service.

### One application registration per customer

Rejected because it creates repeated configuration, credential, redirect, consent, and lifecycle management for every organization.

### Single-tenant application registration

Rejected because it cannot serve users from independent customer Entra tenants.

### Use Entra groups or roles as the Nexus authorization model

Rejected because Nexus owns its product roles, account status, and tenant-specific domain authorization.

### Resolve organizations from email domains

Rejected because email addresses and domains are not a trustworthy tenant boundary. The validated `tid` claim is authoritative.

## Security Requirements

- Use maintained Microsoft identity platform libraries in the Gateway.
- Request only the minimum delegated permissions required.
- Reject tokens with invalid signature, issuer, audience, lifetime, or required claims.
- Bind multitenant issuer validation to the validated `tid`.
- Strip all reserved internal identity and authentication headers received from public clients.
- Keep business microservices on private ingress and prevent routes that bypass the Gateway.
- Use constant-time comparison or a maintained authentication mechanism when validating internal keys.
- Store each service key in GitHub Actions Secrets and inject it only into the Gateway and its intended service.
- Rotate keys without rebuilding container images.
- Do not log tokens, internal keys, or unnecessary personal claims.
- Do not use email as the stable user identity key.
- Record security-relevant organization, consent, role, status, and key-rotation changes.
- Return generic access-denied responses for unknown or disabled tenants and users.
- Test tenant isolation independently from role authorization.
- Define failure behavior for an unavailable Identity service and stale cache entries before production.

## Reconsideration Triggers

Re-evaluate this decision if:

- Microservices become directly accessible outside the trusted internal environment.
- Services are deployed across multiple trust zones or environments.
- Internal shared-key rotation becomes operationally difficult.
- Workload identity, mutual TLS, or a service mesh becomes justified.
- Compliance requires each service to validate the end-user token.
- A customer requires a dedicated identity configuration.
- Additional identity providers become an approved product requirement.
- The frontend changes to a Backend for Frontend or another confidential-client architecture.
- Customer-specific conditional-access or consent requirements cannot be supported by the shared multitenant registrations.

## Validation Criteria

Before production launch, tests must demonstrate that:

1. A user from a registered and enabled Entra tenant can authenticate.
2. Only organizational Microsoft accounts are accepted.
3. A customer tenant can complete the required administrative consent.
4. A first-time valid user is provisioned locally as a Requester.
5. Name and email changes do not change the stable local identity.
6. A user from an unknown or disabled tenant is denied.
7. A locally disabled user is denied even when Entra authentication succeeds.
8. The Gateway rejects invalid signature, issuer, audience, lifetime, and tenant claims.
9. A client cannot override the tenant through routes, bodies, queries, or headers.
10. Client-supplied reserved internal headers are removed.
11. Business microservices cannot be reached directly from the public network.
12. A request without the correct destination-service key is rejected.
13. A key for one microservice cannot authenticate to another microservice.
14. Each microservice enforces its own role and tenant authorization.
15. The Gateway obtains status and roles from Identity and caches them for no more than five minutes.
16. User, role, and organization status changes invalidate the related cache immediately.
17. A failed invalidation never leaves stale authorization context beyond five minutes.
18. Tokens and identities from one tenant cannot retrieve or mutate another tenant's data.
19. GitHub Actions injects service keys without placing them in source control, build logs, or container images.
20. The frontend and API registrations work in at least two independent Entra test tenants.
