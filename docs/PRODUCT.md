# Nexus Support Lite

## Product Specification

**Status:** Architecture synchronized  
**Version:** 2.0  
**Date:** August 1, 2026  
**Related documents:** `PERSONAS.md`, `USER_FLOWS.md`, `SYSTEM_CONTEXT.md`, `CONTAINER_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`

## 1. Vision

Nexus Support Lite is a responsive web application for centralizing and tracking internal support incidents and requests. It prevents cases from being lost across fragmented channels, routes them to the appropriate team, and lets requesters see each case's status and current assignee.

The MVP will be validated with selected organizations. Monetization is not defined at this stage.

## 2. Problem

Incidents arriving through fragmented channels cause lost traceability, unclear ownership, duplicate or unattended handling, poor requester visibility, and difficulty measuring support volume and resolution time.

## 3. MVP Objectives and Success Signals

- Centralize incident registration and tracking.
- Route every incident through a topic with responsible agents.
- Show status, priority, topic, and current assignee.
- Prevent two agents from taking the same incident.
- Preserve an auditable history.
- Validate strict multi-tenant isolation across multiple organizations.
- Collect a baseline for support volume and resolution times.

Success means requesters can follow their incidents, no incident lacks a responsible team, teams can understand workload distribution, and closure time improves after a baseline is established.

## 4. Product Scope

- Responsive web application for desktop and mobile browsers.
- No native mobile application in the MVP.
- Multi-tenant architecture with complete organization isolation.
- Microsoft Entra ID organizational accounts are the only external identity provider in the MVP.
- Nexus uses one shared multitenant frontend App Registration and one shared multitenant API App Registration.
- Organization onboarding requires administrative consent to both applications.
- Knowledge Base and AI-assisted topic or priority suggestions are future capabilities, not MVP features.

## 5. Actors and Permissions

A person may hold multiple roles; the active role determines the interface and available actions.

### Requester

- Create and view their own incidents.
- Select topic and priority manually.
- View status, assignee, comments, attachments, visible history, and resolution.
- Add comments and permitted attachments.

### Agent

- View incidents associated with topics for which they are responsible.
- Atomically take an available incident.
- Change priority when needed.
- Add comments and permitted attachments.
- Delegate only to an active responsible agent of the current topic.
- Transfer an incident to another active topic with mandatory justification.
- Close an assigned incident with a required resolution.

### Organization Administrator

- Manage users, roles, and topic responsibility within the organization.
- Create, edit, activate, and deactivate topics under validated rules.
- View reports and all incidents in the organization.
- Incident access is read-only: administrators cannot comment, assign, delegate, transfer, reprioritize, or close.
- Cannot access another tenant or configure the platform's Entra applications.

### Nexus Global Administrator

- Create and enable organizations.
- Associate an organization with its Microsoft Entra tenant.
- Register the first organization administrator.
- Manage global, tenant-registration, and identity-related settings.
- Cannot view any tenant's incidents, comments, attachments, history, reports, or other operational data.

## 6. Identity and Access

1. The user initiates organizational Microsoft sign-in from Nexus.
2. Microsoft Entra ID authenticates the user through the shared multitenant frontend App Registration.
3. The frontend obtains an access token for the shared API App Registration and sends it to the API Gateway.
4. The Gateway validates signature, issuer, audience, and expiration.
5. Nexus resolves the organization exclusively from the validated `tid` claim. The frontend cannot provide or override the tenant.
6. The Gateway rejects an unknown or disabled organization.
7. Identity resolves the local user, account state, and Nexus roles.
8. On first access, Identity creates the local account with the **Requester** role using validated token claims.
9. The Gateway caches account state and roles for five minutes; relevant changes invalidate that entry immediately.
10. If the account is disabled or has no roles, access is denied or active sessions are closed according to `USER_FLOWS.md`.

Microsoft Entra authentication does not itself grant operational permission; Nexus authorization remains authoritative.

## 7. Topics

A topic has a name, description, active/inactive status, and responsible agents.

- Only active topics may receive new or transferred incidents.
- An active topic must always have at least one responsible agent.
- A topic with open incidents cannot be deactivated until those incidents are transferred.
- Inactive topics remain attached to historical incidents.
- A new incident appears in the shared queue of its topic's responsible agents.
- Those agents receive an in-app notification when the incident is first created.

## 8. Incident Model

Creation data includes the Nexus identifier, subject, description, manually selected topic and priority, organization, requester, status, timestamp, and permitted attachments.

Priorities are **Low**, **Medium**, **High**, and **Critical**.

Operational data includes current assignee, assignment timestamp, chronological comments, attachments, action history, required closure resolution, and closure timestamp.

The lifecycle is:

**New → In Progress → Closed**

- **New:** belongs to a topic, is unassigned, and is available to responsible agents.
- **In Progress:** has exactly one current assignee.
- **Closed:** contains a required resolution.

`In Progress + Unassigned` is invalid. **Resolved**, requester validation, reopening, and additional workflows are future considerations.

## 9. Main Flow

1. The requester enters subject and description, selects an active topic and priority, and optionally adds permitted attachments.
2. Nexus creates the incident as **New** and unassigned.
3. It appears in the topic queue and responsible agents receive in-app notifications.
4. An agent selects **Take incident** from the queue and confirms.
5. Nexus performs an atomic assignment, changes the incident to **In Progress**, and opens its detail.
6. The agent may adjust priority and communicates with the requester through visible chronological comments.
7. The assigned agent enters a required resolution and confirms closure.
8. Nexus changes the incident to **Closed**, preserves history, and notifies the requester.

## 10. Assignment, Delegation, and Transfer

### Taking

- Viewing does not assign an incident.
- Taking requires an explicit confirmation and atomic concurrency control.
- If another agent took it first, Nexus rejects the stale attempt and refreshes the queue.
- Taking opens the detail and does not require priority confirmation.
- Other topic agents are not notified.

### Delegation

- Only the current assignee may delegate.
- Candidates are limited to active responsible agents of the current topic.
- Delegation is immediate; recipient acceptance is not required.
- The incident remains **In Progress** with the new assignee.
- The recipient is notified and full audit history is retained.

### Topic transfer

- The assigned agent chooses an active destination topic and provides mandatory justification.
- Transfer removes the assignee and returns the incident to **New**.
- The requester is notified.
- Destination-topic agents are not notified.
- Previous topic, new topic, assignee, justification, actor, and timestamp are retained.

When an assigned agent loses topic responsibility or is deactivated, affected incidents return to **New** and unassigned while preserving prior work. Notification behavior follows `USER_FLOWS.md`.

## 11. Comments and Attachments

- Comments are chronological, visible to the requester, and record author and timestamp.
- Real-time chat and internal notes are outside the MVP.
- Relevant comments generate in-app notifications according to `USER_FLOWS.md`.
- Attachments are allowed on incidents and comments, limited to lightweight safe types.
- Exact size, formats, storage, malware scanning, retention, and deletion remain pending before implementation.
- Attachments are not a permanent document repository.

## 12. Notifications

Notifications are in-app only; there are no email, SMS, or push notifications in the MVP.

Validated events include:

| Event | Recipient behavior |
| --- | --- |
| Incident created | Notify responsible agents of the topic. |
| Agent takes incident | Do not notify other topic agents. |
| Agent comment | Notify requester. |
| Requester comment on assigned incident | Notify assigned agent. |
| Requester comment on New unassigned incident | Do not notify all topic agents. |
| Priority changed by agent | Notify requester. |
| Delegation | Notify new assignee. |
| Topic transfer | Notify requester; do not notify destination-topic agents. |
| Closure | Notify requester. |
| Assigned agent removed from topic | Notify current responsible agents. |
| Agent deactivated and incidents released | Do not notify topic agents. |

Notifications persist after being marked read. They support read, unread, mark-all-read, a 60-day bell view, and permanent paginated history as defined in `USER_FLOWS.md`.

Notification delivery occurs after the ticket transaction commits. Delivery uses idempotent HTTP requests, Polly resilience, durable pending deliveries, and a Timer-triggered Azure Function. Notification failure never reverses the ticket operation.

## 13. History and Audit

History records actor or system origin, timestamp, and previous/new values when applicable for creation, status and priority changes, assignment, delegation, transfer and justification, comments, attachments, automatic release, closure, and resolution.

Users and topics are deactivated rather than deleted when historical identity must be preserved.

## 14. Views, Filters, and Reports

Minimum incident filters are date range, status, and priority.

- Requesters see their own incidents.
- Agents see incidents associated with their topics.
- Organization administrators have read-only access to all tenant incidents.
- No role can access another tenant's operational content.

Basic reports cover status, topic, priority, assigned agent, and period, plus an informational count of incidents closed by each agent. This KPI is not a standalone performance or quality measure.

Text search and advanced reporting are outside the MVP.

## 15. Essential Non-Functional Requirements

### Isolation and security

- Every operation, query, and notification is scoped to trusted tenant context.
- The tenant is derived only from validated `tid`.
- Authorization uses Nexus role, topic membership, assignment, account state, and tenant.
- Internal services are private and apply their own functional authorization.
- Attachment access, type, size, and malware controls must be defined before release.
- Relevant actions are auditable.

### Usability and availability

- Incident registration is brief and understandable.
- Status, priority, topic, and **Assigned to** are visible.
- The UI adapts to desktop and mobile browsers.
- Atomic taking prevents duplicate assignment.
- Notification outages cannot block ticket operations.

## 16. Explicitly Out of Scope

- Knowledge Base and AI-assisted classification.
- AI services, providers, models, indexes, embeddings, or vector stores.
- Monetization and commercial plans.
- Additional identity providers.
- Email/Teams/other automatic intake.
- External notification channels.
- Native mobile applications.
- Real-time chat and internal notes.
- SLA management and satisfaction surveys.
- Text search and advanced reports.
- **Resolved**, requester closure validation, and reopening.
- Attachments as permanent document storage.
- Azure Service Bus or another message broker.

## 17. High-Level Acceptance Criteria

1. An organizational Entra user can access Nexus only when `tid` maps to an enabled organization.
2. A first-time local user is created as Requester.
3. A requester can create an incident by manually selecting topic and priority.
4. A new incident is visible only to responsible agents and notifies them.
5. Exactly one concurrent take succeeds.
6. Taking opens the detail and does not force a priority action.
7. Every In Progress incident has one assignee.
8. Delegation is immediate and limited to current-topic responsible agents.
9. Transfer requires justification, returns the incident to New, and notifies the requester but not destination agents.
10. Only the assignee can close; resolution and confirmation are required.
11. Administrators can inspect but cannot handle incidents.
12. History identifies every relevant action and time.
13. Notification history and read state persist.
14. Ticket operations remain successful when Notifications is unavailable.
15. No user or administrator crosses tenant boundaries.
16. No Knowledge Base, AI, broker, or additional IdP is required for MVP acceptance.

## 18. Decisions Pending Before Implementation

- Retention policies for incidents, history, attachments, notifications, and retry records.
- Maximum attachment size and permitted formats.
- Attachment storage and malware scanning.
- Exact API payloads and notification content minimization.
- Retry Function authentication to Notification Service.
- Visual guidance for manual priority selection.
- Required organization-onboarding fields beyond the Entra tenant association.
- Initial observation period for the closure-time baseline.

Identity provider, tenant resolution, storage ownership, retry architecture, and AI exclusion are already decided.

## 19. Risks and Mitigations

| Risk | Mitigation |
| --- | --- |
| Broad access after first login | Nexus validates enabled tenant, local account state, and roles; organizations control Entra membership and consent. |
| Cross-tenant exposure | Tenant comes only from validated `tid`; every domain enforces isolation. |
| Two agents take one case | Atomic assignment and concurrency control. |
| Notification outage | Commit ticket first; use idempotency, Polly, durable retry, and operational visibility. |
| Notification fatigue | In-app only and limited by validated event rules. |
| Attachments become a repository | Limit types/size and apply lifecycle and retention policy. |
| KPI becomes performance scoring | Keep it informational and contextual. |

## 20. Potential Future Evolution

- Knowledge Base and human-controlled AI suggestions.
- **Resolved**, requester confirmation, and reopening.
- Automatic intake from other channels.
- SLA management, surveys, search, advanced reports, and configurable notifications.
- Additional identity providers.
- Broker-based asynchronous integration if scale or delivery requirements justify it.
