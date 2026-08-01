# Nexus Support Lite

## System Context

**Status:** Discovery validated  
**Version:** 1.0  
**Date:** August 1, 2026  
**Related documents:** `PRODUCT.md`, `PERSONAS.md`, `USER_FLOWS.md`

## 1. Purpose

This document defines the system boundary of Nexus Support Lite, the people who interact with it, the external systems required by the initial version, and the principal relationships between them.

It does not define internal applications, services, databases, deployment units, or implementation technologies. Those decisions belong in `CONTAINER_DIAGRAM.md`, `DEPLOYMENT_DIAGRAM.md`, and the relevant Architecture Decision Records.

## 2. System of Interest

Nexus Support Lite is a multi-tenant web platform that centralizes and tracks internal IT support incidents for organizations from different industries.

Each organization operates within an isolated tenant. Its users, roles, topics, incidents, comments, attachments, history, notifications, configurations, and reports must remain inaccessible to every other organization.

## 3. People

The human context, goals, frustrations, and usage patterns of the primary users are defined in `PERSONAS.md`. Their permissions and detailed interactions are defined in `PRODUCT.md` and `USER_FLOWS.md`.

| Person | Relationship with Nexus Support Lite |
| --- | --- |
| Requester | Registers incidents and follows their status, comments, assignee, and resolution within their organization. |
| Agent | Handles incidents associated with their assigned topics and records the work performed. |
| Organization Administrator | Maintains users, roles, and topics and consults operational information within their organization. |
| Nexus Global Administrator | Onboards and enables organizations and configures their identity-provider connection. This person belongs to the Nexus provider and cannot access tenant operational data. |

## 4. External Systems

### 4.1 Microsoft Entra ID

Microsoft Entra ID is the only identity provider integrated in the initial phase.

For each enabled organization, Entra ID:

- Authenticates users belonging to that organization's tenant.
- Returns the tenant identifier used by Nexus to identify the organization.
- Provides the authenticated user's basic profile, initially name and email address.

Nexus trusts the authentication performed by the configured identity provider but remains responsible for tenant resolution, local account status, role authorization, topic authorization, and data isolation.

### 4.2 Future Identity Providers

The product is expected to support additional identity providers in later phases. This is an architectural extensibility requirement, not an initial external integration. No provider other than Microsoft Entra ID is selected or implemented in the first phase.

### 4.3 User Web Browser

All people access Nexus through a web browser. The product is a responsive web application; native mobile applications are outside the initial scope.

### 4.4 Systems Explicitly Outside the Initial Context

The initial version has no integration with email, Microsoft Teams, or external messaging systems. Notifications are generated and consumed only within Nexus.

The initial version also does not ingest incidents automatically from Teams, email, or other external channels.

## 5. Context Diagram

```mermaid
flowchart TB
    Users["Organization users<br/>Requester · Agent · Administrator"]
    GlobalAdmin["Nexus Global Administrator"]
    Nexus["Nexus Support Lite<br/>Multi-tenant support platform"]
    Entra["Microsoft Entra ID<br/>Initial identity provider"]

    Users -->|"Use support and administration functions"| Nexus
    GlobalAdmin -->|"Onboard organizations and configure identity"| Nexus
    Nexus -->|"Redirect for authentication"| Entra
    Entra -->|"Return identity, tenant ID, name, and email"| Nexus
```

The diagram shows only the initial external context. Future identity providers are excluded because none has yet been selected.

## 6. Organization Onboarding and Access Context

1. The Nexus Global Administrator registers and enables an organization before its users can access Nexus.
2. The administrator configures the organization's Microsoft Entra ID tenant.
3. A user authenticates through that tenant's Entra ID connection.
4. Nexus identifies the organization from the tenant ID returned during authentication.
5. Nexus rejects access when the tenant ID is unknown or the organization is disabled and informs the user that the organization is not enabled.
6. Any successfully authenticated user from an enabled tenant may enter Nexus without prior manual creation by an organization administrator.
7. On first access, Nexus creates the local user automatically with the **Requester** role.
8. Nexus synchronizes the user's name and email address from Entra ID during sign-in.
9. Subsequent access and actions remain subject to the local account status, assigned roles, topic membership, and tenant authorization rules defined in `USER_FLOWS.md`.

## 7. Trust and Security Boundaries

### 7.1 Identity Boundary

- Each organization uses its own identity-provider configuration.
- Microsoft Entra ID authenticates the person; Nexus authorizes what that person may do.
- An authenticated identity alone does not grant access to an unregistered or disabled organization.
- A locally deactivated user must not retain access even if Entra ID authenticates them successfully.

### 7.2 Tenant Boundary

- The authenticated tenant context must be established before operational data is accessed.
- Every request and data operation must be constrained to the active organization.
- Tenant users and administrators cannot access another organization's operational or configuration data.
- The Nexus Global Administrator can manage tenant and identity configuration but cannot view incidents, comments, attachments, histories, notifications, or reports belonging to an organization.

### 7.3 Application Boundary

- Incident data, notification delivery, notification history, and user-facing audit information remain inside Nexus in the initial version.
- The browser is an untrusted client; tenant and authorization checks must be enforced by Nexus rather than relying on the user interface.

## 8. Key Context-Level Requirements

1. Nexus must support multiple organizations on the same platform with complete tenant isolation.
2. Each organization must have an independently configured identity-provider connection.
3. Microsoft Entra ID must be supported in the initial phase.
4. The architecture must permit additional identity providers without making them part of the initial implementation scope.
5. The organization must be resolved from the authenticated tenant ID.
6. Unknown or disabled organizations must be denied access.
7. A valid first-time user from an enabled tenant must be provisioned automatically as a Requester.
8. Name and email must be synchronized from Entra ID during sign-in.
9. Authentication, local user status, role authorization, topic authorization, and tenant authorization must remain distinct controls.
10. Initial notifications must remain internal to Nexus.
11. Identity-provider failure must prevent authentication but must not compromise or expose tenant information.

## 9. Context-Level Constraints and Non-Decisions

- No additional identity provider has been selected for implementation.
- No email or Teams integration is included in the initial version.
- No decision is made here about a backend architecture, API style, database, cloud provider, hosting model, or identity library.
- No decision is made here about whether tenants share physical infrastructure or storage. Only logical isolation is mandatory at this level.
- AI is an internal product capability from the user's perspective. Its provider and integration boundary remain pending technical design and should be resolved in the container design or an ADR.

## 10. Required Synchronization with Existing Documentation

The following newer Discovery decisions supersede or clarify portions of `PRODUCT.md` and must be synchronized there before implementation:

- Microsoft Entra ID is the sole identity provider integrated in the initial phase, while future multi-IdP support remains an architectural requirement.
- Each organization uses its own identity-provider configuration.
- The Nexus Global Administrator must register and enable the organization and configure its identity connection before access is allowed.
- Nexus identifies the organization from the tenant ID returned during authentication.
- An unknown tenant ID or disabled organization causes access to be rejected.
- The earlier email-first organization-resolution flow in `PRODUCT.md` is not the current decision for the Entra ID initial flow and requires revision.
- Nexus synchronizes the user's name and email from Entra ID during sign-in.

## 11. Context-Level Acceptance Criteria

1. A user authenticated by the configured Entra ID tenant of an enabled organization is associated only with that organization.
2. A tenant ID that is not registered cannot access Nexus.
3. A disabled organization cannot access Nexus even when Entra ID authentication succeeds.
4. A valid first-time user is created automatically as a Requester with the name and email supplied by Entra ID.
5. A locally deactivated user cannot access Nexus even when the external identity remains valid.
6. Operational data from one organization is never returned to a user, administrator, or request associated with another organization.
7. The Nexus Global Administrator can manage organization and identity configuration without access to tenant operational data.
8. Users receive product notifications within Nexus and no notification is sent through email or Teams.

## 12. Next Architecture Document

The next recommended document is `CONTAINER_DIAGRAM.md`. It should define the major executable and data containers inside the Nexus boundary, their responsibilities, and their communication paths while preserving the identity and tenant boundaries established here.
