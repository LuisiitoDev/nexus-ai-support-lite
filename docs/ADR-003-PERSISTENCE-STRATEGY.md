# ADR-003: Use Polyglot Persistence with Isolated Service Databases

**Status:** Accepted  
**Date:** August 1, 2026  
**Decision owners:** Nexus Support Lite team  
**Related documents:** `CONTAINER_DIAGRAM.md`, `DEPLOYMENT_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`, `ADR-001-AZURE-CONTAINER-APPS.md`, `ADR-002-MULTITENANT-IDENTITY.md`

## Context

Nexus Support Lite is a multi-tenant SaaS platform composed of independently deployable microservices. Each domain must own its data and prevent direct database access by other services.

The MVP contains three data-owning services:

- **Identity**, which manages organizations, users, roles, and account status.
- **Tickets**, which manages tickets, topics, assignments, comments, states, and priorities.
- **Notifications**, which manages each user's notification history and read state.

Identity and Tickets have relational data and transactional consistency requirements. Notifications has a document-oriented, user-centered access pattern and may grow independently.

The MVP must preserve domain isolation while limiting Azure cost and operational overhead.

## Decision

Use polyglot persistence with one independently owned database per microservice:

- **Identity:** Azure SQL Database.
- **Tickets:** Azure SQL Database.
- **Notifications:** Azure Cosmos DB for NoSQL.

Identity and Tickets will have separate databases. For the MVP, those databases may share the same Azure SQL logical server to reduce cost and administration. Sharing a logical server does not authorize cross-database access.

Each service is the exclusive owner of its database. Services must integrate through their published APIs and must not read, write, join, query, or share tables or containers owned by another service.

## Multi-Tenant Data Model

Each service uses a database shared by all customer organizations. The MVP will not create a database or Cosmos DB account per customer.

Tenant isolation is logical:

- Tenant-owned relational records include a non-null `TenantId`.
- Queries and mutations derive the tenant from the trusted context established by the API Gateway.
- A client-provided tenant identifier is never authoritative.
- Database access code must apply the tenant boundary consistently.
- Uniqueness constraints and indexes must include `TenantId` when uniqueness is tenant-scoped.

Platform-wide records that are intentionally not tenant-owned must be explicitly identified and protected by separate authorization rules.

## Azure SQL Isolation

Identity and Tickets use different Azure SQL databases, schemas, migration histories, and managed identities.

Each Azure Container App receives its own managed identity:

- The Identity managed identity receives permissions only on the Identity database.
- The Tickets managed identity receives permissions only on the Tickets database.
- Neither identity receives permissions on the other service's database.

Application workloads authenticate to Azure SQL through Microsoft Entra ID. Database usernames, passwords, and SQL authentication connection secrets are not used by the running services.

The CI/CD migration principal is separate from the runtime identities and receives only the schema permissions required for the target database.

## Cosmos DB Partitioning and Consistency

Notifications uses an Azure Cosmos DB for NoSQL container with a hierarchical partition key:

1. `/tenantId`
2. `/userId`

Every notification document includes both values. This design aligns the partition boundary with the two primary access dimensions: organization and recipient.

The service must query notifications using both `TenantId` and `UserId` whenever the operation is user-specific. Cross-tenant queries are prohibited outside explicitly approved platform operations.

Cosmos DB uses **Session consistency**. A user therefore observes their own notification writes and state changes consistently within their session without paying the latency and availability trade-offs of Strong consistency.

Notifications authenticates to Cosmos DB through its own Azure Container Apps managed identity. Account keys and secret-bearing connection strings are not used by the running service. The identity receives only the Cosmos DB data-plane role required by Notifications.

## Notification History and Retention

Marking a notification as read changes its state; it does not delete the notification.

Read and unread notifications remain available as persistent history. A retention or archival policy will be defined later using observed volume, product requirements, compliance obligations, and cost. Until that policy is approved, the implementation must not assume that read notifications can be discarded.

## Schema Migration Strategy

Azure SQL migrations for Identity and Tickets run as explicit, controlled CI/CD pipeline steps.

The application services do not apply migrations automatically at startup.

The deployment workflow must:

1. Build and test the application and migration artifacts.
2. Execute the migration for the target service and environment using its migration identity.
3. Verify that the migration completed successfully.
4. Stop the deployment if the migration fails.
5. Continue deployment only after successful verification.

Migration execution must be observable and auditable. Migration scripts must be idempotent where practical and designed for backward compatibility during rolling or revision-based deployments.

Cosmos DB container, indexing, partition-key, and throughput changes must also be managed as versioned infrastructure or deployment changes rather than ad hoc production edits.

## Decision Drivers

- Preserve exclusive data ownership per bounded context.
- Use relational storage for transactional domains.
- Use document storage for user-centered notification history.
- Support multi-tenancy without a database per customer.
- Keep MVP infrastructure cost and administration manageable.
- Prevent one compromised service identity from accessing another service's data.
- Eliminate database passwords from application runtime configuration.
- Control schema changes independently from application startup.
- Support independent scaling and evolution of Notifications.

## Consequences

### Positive

- Domain ownership is explicit and enforceable.
- Identity and Tickets can evolve their schemas independently.
- Sharing one Azure SQL logical server reduces MVP overhead without sharing databases.
- Managed identities remove application database passwords and support least privilege.
- Cosmos DB partitioning aligns notification access with tenant and user boundaries.
- Session consistency supports immediate user-visible state changes at a lower trade-off than Strong consistency.
- Controlled migrations provide deployment traceability and predictable failure handling.
- Persistent notification history supports auditability and future product features.

### Negative and trade-offs

- The system operates two database technologies.
- Local development, observability, backups, and incident response must cover both Azure SQL and Cosmos DB.
- Cross-domain joins are unavailable and must be replaced by API composition or replicated read models when justified.
- Logical multi-tenant isolation requires disciplined query filtering and automated tests.
- Sharing an Azure SQL logical server creates a shared infrastructure dependency even though the databases are isolated.
- Hierarchical partition-key selection is difficult to change after production adoption.
- Persistent notification history can grow indefinitely until a retention policy is approved.
- Controlled migrations add pipeline complexity and require compatibility planning across application revisions.
- Managed identity configuration requires Azure role assignments and environment-specific provisioning.

## Alternatives Considered

### One shared relational database for all microservices

Rejected because it weakens domain ownership, enables direct cross-service coupling, and makes independent evolution and least-privilege access difficult.

### One database per customer

Rejected for the MVP because it increases provisioning, migration, monitoring, backup, and cost overhead. The selected model uses shared service databases with logical isolation by `TenantId`.

### One Azure SQL database with a schema per service

Rejected because schema separation does not provide the same ownership and permission boundary as separate databases.

### Azure SQL for Notifications

Rejected because the notification workload is naturally document-oriented, user-centered, and independently scalable. Cosmos DB better matches the selected access pattern.

### Cosmos DB for all services

Rejected because Identity and Tickets have relational constraints and transactional workflows that are better served by Azure SQL.

### SQL usernames and passwords

Rejected because Azure Container Apps managed identities and Microsoft Entra authentication avoid long-lived application database credentials.

### Automatic migrations at application startup

Rejected because startup-time migrations can cause concurrent execution, obscure failures, and couple schema modification to application availability.

### Delete notifications when read

Rejected because read status is a state transition, not a deletion event. History will be retained until a separate retention policy is approved.

### Strong Cosmos DB consistency

Rejected for the MVP because Session consistency satisfies the user-facing read-after-write requirement with fewer latency and availability trade-offs.

## Security Requirements

- Derive `TenantId` and `UserId` from trusted authenticated context, never from untrusted client input.
- Apply tenant filters to every tenant-owned query and mutation.
- Include tenant scope in tenant-specific uniqueness rules.
- Use separate managed identities for Identity, Tickets, and Notifications.
- Restrict each runtime identity to the minimum data-plane permissions on its own database.
- Prevent application runtime identities from modifying database schemas.
- Use separate, narrowly scoped identities for CI/CD migrations.
- Do not store SQL passwords, Cosmos DB keys, or secret-bearing connection strings in source control, container images, logs, or application configuration.
- Record and monitor authorization failures, cross-tenant access attempts, migration failures, and database permission changes.
- Encrypt data in transit and use Azure-managed encryption at rest.
- Back up and restore each service's data independently.
- Treat cross-tenant operational access as a privileged, audited platform capability.
- Test tenant isolation at repository, service, and integration levels.

## Operational Requirements

- Monitor Azure SQL availability, resource consumption, connection failures, deadlocks, and migration execution.
- Monitor Cosmos DB request units, throttling, latency, storage growth, partition distribution, and failed requests.
- Alert on repeated cross-tenant authorization failures and unexpected access-denied responses from managed identities.
- Define independent backup, restore, disaster-recovery, and data-export procedures for each service.
- Verify that restoring one service does not require direct mutation of another service's database.
- Review notification volume before selecting a retention or archival policy.

## Reconsideration Triggers

Re-evaluate this decision if:

- A customer contract or regulation requires physical database isolation.
- Tenant volume or noisy-neighbor behavior makes shared databases unsuitable.
- Azure SQL databases require independent logical servers for security, scaling, regional, or operational reasons.
- Notification access patterns no longer center on tenant and user.
- Cosmos DB partition distribution becomes uneven or produces hot partitions.
- Notification history cost requires an approved retention or archival policy.
- Cross-service reporting requires a dedicated analytics or read-model solution.
- Managed identity support or network topology changes materially.
- The operational cost of two database technologies outweighs their workload fit.

## Validation Criteria

Before production launch, tests and deployment evidence must demonstrate that:

1. Identity and Tickets use separate Azure SQL databases.
2. The databases may share a logical server without granting cross-database permissions.
3. Identity cannot access the Tickets database.
4. Tickets cannot access the Identity database.
5. Notifications cannot access either Azure SQL database.
6. Each service authenticates to its database using its own managed identity.
7. Runtime services operate without SQL passwords, Cosmos DB keys, or secret-bearing connection strings.
8. A tenant cannot read or mutate another tenant's relational records.
9. Tenant-scoped uniqueness and indexes include `TenantId` where required.
10. Notifications stores `tenantId` and `userId` and uses them as the hierarchical partition key.
11. A user cannot read or update another user's notifications without an explicitly authorized domain operation.
12. Cosmos DB uses Session consistency.
13. Marking a notification as read preserves the document and its history.
14. Azure SQL migrations run through controlled CI/CD steps.
15. A failed migration stops deployment.
16. Identity and Tickets do not apply migrations at application startup.
17. Migration identities cannot access databases outside their target service.
18. Runtime identities cannot perform unauthorized schema changes.
19. Cosmos DB infrastructure and indexing changes are versioned and repeatable.
20. Backup and restore procedures are verified independently for Identity, Tickets, and Notifications.
