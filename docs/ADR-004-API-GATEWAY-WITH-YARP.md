# ADR-004: Use ASP.NET Core and YARP as the API Gateway

**Status:** Accepted  
**Date:** August 1, 2026  
**Decision owners:** Nexus Support Lite team  
**Related documents:** `CONTAINER_DIAGRAM.md`, `DEPLOYMENT_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`, `ADR-001-AZURE-CONTAINER-APPS.md`, `ADR-002-MULTITENANT-IDENTITY.md`, `ADR-003-PERSISTENCE-STRATEGY.md`

## Context

Nexus Support Lite requires a single public entry point for its backend services. The API Gateway must route requests to independently deployable microservices while enforcing the authentication and trusted identity propagation model established in ADR-002.

The MVP prioritizes low infrastructure cost, limited operational overhead, compatibility with the existing .NET expertise, and deployment on Azure Container Apps. The backend services are known in advance and do not require a dynamic service-discovery platform.

The Gateway implementation must not constrain downstream microservices to one programming language. A service may use .NET, Python, or another suitable technology as long as it complies with the platform's HTTP contracts, trusted identity headers, authorization responsibilities, and network controls.

## Decision

Implement the API Gateway as an **ASP.NET Core application using YARP (Yet Another Reverse Proxy)** and deploy it as an Azure Container App.

The Gateway is the only backend component with public ingress. All downstream microservices use internal ingress and must not be directly reachable from the public internet.

YARP routes and clusters will be defined through static configuration versioned in the repository, initially using `appsettings.json` and environment-specific configuration overrides where required. Dynamic service discovery is outside the MVP.

The Gateway will apply configurable rate limiting by `TenantId`. Concrete limits will be selected through load testing and operational observation before production launch.

Per-user rate limiting is explicitly outside the MVP scope.

## Gateway Responsibilities

The Gateway is responsible for:

- Serving as the single public backend entry point.
- Validating Microsoft Entra ID access tokens.
- Rejecting requests from unregistered or disabled tenants.
- Resolving the organization exclusively from the validated `tid` claim.
- Consulting Identity for the current user state and Nexus-managed roles.
- Caching user state and roles for five minutes, with immediate invalidation when those values change.
- Removing any client-supplied identity headers before proxying a request.
- Adding trusted internal headers containing the validated user identity, `TenantId`, and roles.
- Adding the service-specific internal key required by the destination microservice.
- Routing requests to the appropriate internal service.
- Applying configurable rate limiting by `TenantId`.
- Returning consistent gateway-level responses for authentication, routing, and throttling failures.
- Producing correlation and observability data for proxied requests.

## Microservice Responsibilities

Each microservice remains responsible for:

- Verifying its own service-specific internal key.
- Rejecting traffic that does not originate through the approved internal path.
- Treating trusted identity headers as authoritative only after internal-key verification.
- Enforcing functional authorization according to its domain rules.
- Applying tenant isolation to every tenant-owned operation.
- Validating request data and executing business logic.
- Exposing health endpoints required by the platform.

The microservices will not duplicate Microsoft Entra ID token validation. This decision depends on network isolation, private ingress, identity-header sanitization, and service-specific internal keys preventing callers from bypassing the Gateway.

## Routing Configuration

Routes and destination clusters are statically declared and version controlled.

Configuration changes must:

1. Be reviewed like application code.
2. Be validated in CI before deployment.
3. Support environment-specific internal service addresses without storing secrets in source control.
4. Preserve stable public API paths when internal services or revisions change.
5. Be deployed through the standard CI/CD process.

The MVP will not introduce a service registry or runtime discovery mechanism. Azure Container Apps internal DNS names provide stable destinations for the known services.

## Polyglot Microservices

YARP proxies HTTP traffic and does not require downstream services to use ASP.NET Core.

A microservice may be implemented in Python or another language when that technology is appropriate. Every implementation must honor the same platform contracts:

- HTTP API contract and versioning rules.
- Internal-only ingress.
- Trusted identity-header names and semantics.
- Service-specific internal-key validation.
- Tenant and functional authorization requirements.
- Health, telemetry, correlation, and error-handling conventions.

Knowledge Base and AI remain outside the MVP, but a future service in that domain may use Python without changing the Gateway decision.

## Tenant Rate Limiting

Rate limiting is applied after the token has been validated and the tenant has been resolved from the trusted `tid` claim.

The policy must:

- Partition limits by `TenantId`.
- Reject attempts to select a rate-limit partition through client-provided tenant data.
- Use configuration rather than hard-coded thresholds.
- Return HTTP `429 Too Many Requests` when a tenant exceeds its limit.
- Provide an appropriate `Retry-After` response when possible.
- Emit metrics and logs for throttled requests without exposing sensitive token or identity data.
- Be validated with load tests before production limits are approved.

Per-user rate limiting may be reconsidered later if one user can exhaust a tenant's capacity or if product tiers require user-level quotas.

## Decision Drivers

- Provide one controlled public entry point.
- Reuse the team's .NET expertise.
- Support the authentication and trusted-header model from ADR-002.
- Avoid the additional service cost and administration of Azure API Management for the MVP.
- Keep routing configuration simple, explicit, and auditable.
- Support independently deployable and polyglot microservices.
- Prevent one tenant from monopolizing shared system capacity.
- Preserve each microservice's ownership of functional authorization.
- Avoid dynamic service-discovery complexity while the service set is small and known.

## Consequences

### Positive

- YARP integrates naturally with ASP.NET Core authentication, middleware, dependency injection, configuration, logging, and rate-limiting capabilities.
- The Gateway can implement the exact token-validation and header-transformation behavior already selected.
- Static route configuration is easy to review, test, and version.
- The MVP avoids an additional managed gateway service and its associated cost.
- Only one backend application requires public ingress.
- Downstream services remain free to use .NET, Python, or another suitable technology.
- Tenant-level rate limiting protects shared capacity across organizations.
- Microservices keep domain-specific authorization close to their business rules.

### Negative and trade-offs

- The team owns Gateway availability, scaling, patching, security, and configuration.
- A Gateway failure can affect all backend traffic.
- Static routes require a deployment when service topology or routing changes.
- The Gateway adds a network hop and must be monitored for latency and saturation.
- Centralized token validation makes network isolation and internal-key enforcement critical security controls.
- Internal shared keys require secure generation, storage, rotation, and service-specific configuration.
- Tenant-level rate limiting does not prevent a single user from exhausting the quota of their organization.
- YARP does not provide every governance, developer-portal, subscription, or analytics feature available in Azure API Management.
- Supporting polyglot services requires language-neutral contracts and equivalent security implementations.

## Alternatives Considered

### Azure API Management

Deferred for the MVP because the selected requirements can be implemented with YARP, while API Management introduces another managed service, configuration surface, and cost.

Reconsider API Management if the platform requires external developer onboarding, subscriptions and API keys, monetization, advanced policies, a developer portal, complex API governance, or managed analytics that would otherwise be rebuilt.

### Azure Front Door as the application Gateway

Rejected as the sole API Gateway because edge routing does not replace the application-level Microsoft Entra validation, Identity lookup, trusted-header transformation, and domain-aware tenant controls required by Nexus. Front Door may still be introduced later as an edge service.

### Direct public ingress for every microservice

Rejected because it would expand the public attack surface, duplicate cross-cutting authentication concerns, and allow callers to bypass the trusted Gateway path.

### Dynamic service discovery

Rejected for the MVP because the set of services is small and known, and Azure Container Apps provides stable internal DNS destinations. The added complexity is not currently justified.

### Validate Microsoft Entra tokens in every microservice

Rejected by ADR-002 to avoid duplicating authentication work. The selected model centralizes token validation in the Gateway and compensates with private service ingress, trusted-header sanitization, and distinct internal keys.

### Forward the original access token to every microservice

Rejected by ADR-002. The Gateway sends sanitized internal identity headers after validation instead.

### Rate limiting by user

Deferred outside the MVP scope. Tenant-level limiting addresses the initial noisy-neighbor risk with less policy and state complexity.

## Security Requirements

- Only the Gateway may expose public backend ingress.
- Downstream microservices must use internal ingress and network controls that prevent public access.
- The Gateway must validate token signature, issuer, audience, lifetime, and required claims.
- The tenant must come exclusively from the validated `tid` claim.
- The Gateway must strip all identity and internal-key headers received from external clients.
- The Gateway must generate trusted identity headers only after successful authentication and authorization-context lookup.
- Each downstream service must use a different internal key.
- Internal keys must be stored as deployment secrets, never in source control, images, logs, or static route configuration.
- Keys must be rotatable without changing application code.
- Each microservice must verify its internal key before trusting identity headers.
- Rate-limit partitioning must use the trusted tenant context.
- Logs must not contain access tokens, internal keys, or unnecessary personal data.
- TLS must protect external and internal traffic.
- Functional authorization remains mandatory in each microservice.
- Any future non-.NET service must implement equivalent internal-key and trusted-header controls.

## Operational Requirements

- Deploy at least one independently scalable Gateway container app revision.
- Define readiness and liveness health checks.
- Monitor request count, latency, upstream latency, failure rate, throttling, authentication failures, route failures, and resource saturation.
- Propagate or generate a correlation identifier for every request.
- Validate route and cluster configuration during CI.
- Test every configured route against the intended internal destination.
- Keep environment-specific destination addresses outside hard-coded application logic.
- Establish rollback procedures for invalid route configuration.
- Load-test the Gateway before approving production rate limits and scaling rules.
- Alert on unusual authentication failures, rejected internal-key checks, throttling spikes, and attempts to access internal services directly.
- Document internal-key rotation and emergency revocation procedures.

## Reconsideration Triggers

Re-evaluate this decision if:

- API governance, subscriptions, monetization, transformation policies, or a developer portal become product requirements.
- Gateway maintenance becomes a disproportionate operational burden.
- Static route changes become frequent or the service topology becomes highly dynamic.
- The number of services materially increases.
- Multi-region traffic management or edge protection requires additional managed infrastructure.
- Gateway throughput, latency, or availability targets cannot be met economically.
- Tenant-level rate limiting is insufficient and per-user or product-tier policies are required.
- The internal-key model no longer provides an acceptable service-to-service trust boundary.
- A service mesh or zero-trust workload identity model is adopted.
- Direct non-HTTP communication patterns become a significant part of the architecture.

## Validation Criteria

Before production launch, tests and deployment evidence must demonstrate that:

1. The Gateway runs as an ASP.NET Core application using YARP in Azure Container Apps.
2. The Gateway is the only backend component with public ingress.
3. Identity, Tickets, and Notifications cannot be accessed directly from the public internet.
4. Every public API route maps to the intended internal service.
5. Route and cluster configuration is version controlled and validated in CI.
6. The MVP operates without dynamic service discovery.
7. The Gateway validates Microsoft Entra ID tokens according to ADR-002.
8. Client-supplied identity and internal-key headers are removed.
9. Trusted identity headers are added only after successful validation.
10. Each service receives and verifies its own distinct internal key.
11. A request that bypasses the Gateway is rejected.
12. Each microservice enforces its own functional authorization rules.
13. A service implemented in Python can comply with the same language-neutral security and routing contract.
14. Rate limiting partitions requests using the trusted `TenantId`.
15. One tenant's rate-limit consumption does not reduce another tenant's configured allowance.
16. A throttled request receives HTTP `429` and an appropriate retry indication when supported.
17. Rate-limit thresholds can change through configuration.
18. Per-user rate limiting is not required for MVP acceptance.
19. Gateway health checks and telemetry are visible in the operational environment.
20. Correlation identifiers allow a request to be traced from the Gateway to the destination service.
