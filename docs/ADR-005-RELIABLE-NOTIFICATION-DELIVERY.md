# ADR-005: Reliable Ticket-to-Notification Delivery Without a Broker

- **Status:** Accepted
- **Date:** August 1, 2026
- **Decision owners:** Nexus Support Lite architecture
- **Related:** `CONTAINER_DIAGRAM.md`, `DEPLOYMENT_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`, `ADR-002-MULTITENANT-IDENTITY.md`, `ADR-003-PERSISTENCE-STRATEGY.md`

## Context

Ticket operations can require in-app notifications. The MVP must preserve the ticket result when Notifications is unavailable, avoid duplicate notifications during retries, and recover pending deliveries after workload restarts.

Azure Service Bus and other message brokers are explicitly outside the MVP. Therefore, reliability must be implemented through HTTP communication and durable Tickets-owned retry state.

## Decision

### Commit before notification

Ticket Service commits its own database transaction before calling Notification Service. A notification failure never rolls back or invalidates the ticket operation.

### Internal HTTP delivery

Ticket Service calls Notification Service through internal HTTP. Immediate calls use Polly policies with:

- A controlled timeout.
- Exponential-backoff retries.
- A configurable maximum number of attempts.
- A circuit breaker for consecutive failures.

When the circuit breaker is open, delivery attempts that cannot be made immediately are recorded as pending.

### Idempotency

Every notification request includes a unique operation identifier. Notification Service persists or otherwise recognizes that identifier and must not create a duplicate notification when the same operation is retried.

### Durable pending deliveries

If immediate delivery fails after the configured policies, Ticket Service stores a pending-delivery record in Ticket Database. The record includes at least:

- Unique operation identifier.
- Tenant and recipient context required by the delivery contract.
- Notification payload or safe reference required to recreate the request.
- Attempt count.
- Next-attempt timestamp.
- Last error.
- Delivery state.

Pending-delivery state belongs to the Tickets domain. Successful notification history and read/unread state belong to the Notifications domain.

### Azure Function retry processor

A Timer-triggered Azure Function processes pending deliveries. Its initial schedule is every minute and remains configurable.

The Function:

1. Uses its own managed identity.
2. Receives least-privilege access to pending-delivery data in Ticket Database.
3. Claims eligible work safely so concurrent executions do not process the same delivery simultaneously.
4. Calls Notification Service with the original operation identifier.
5. Applies the configured Polly resilience policies.
6. Updates the attempt count, next-attempt time, last error, and state.
7. Marks a delivery as `Failed` after the configurable maximum attempts for manual review.

The Function is a deployable component within the Tickets domain. Its access to Ticket Database does not cross a domain boundary.

### Authentication left explicit

The mechanism by which the Retry Function authenticates its HTTP request to Notification Service is **TBD**. It will not validate an Entra token merely because the Function has a managed identity, and it will not silently reuse the Gateway's end-user trust flow. This must be decided before implementation.

## Consequences

### Positive

- Ticket availability is independent of Notification Service availability.
- The system avoids duplicate notifications during retries.
- Pending work survives Ticket Service and Function restarts.
- No message broker is required for the MVP.
- Failures are visible and recoverable through durable state.
- Ticket and Notifications data ownership remains separate.

### Negative

- Tickets owns additional delivery-state tables and processing logic.
- The Azure Function introduces another deployable workload.
- Polling may delay notifications and creates database load.
- The solution recreates a limited reliable-delivery mechanism that a broker could otherwise provide.
- Manual handling is required for deliveries ending in `Failed`.
- Function-to-Notifications authentication remains unresolved.

## Alternatives Considered

### Direct HTTP without durable retry

Rejected because notifications could be permanently lost after immediate attempts or a process restart.

### BackgroundService inside Ticket Service

Rejected in favor of an Azure Function so retry processing has an independent execution lifecycle.

### Azure Service Bus or another broker

Deferred outside the MVP to control cost and operational scope. It may be reconsidered if fan-out, throughput, ordering, delivery guarantees, or multiple consumers justify it.

### Roll back the ticket when notification delivery fails

Rejected because notification availability must not control the success of the business operation.

## Operational Requirements

- Polly thresholds and delays are configurable.
- Logs and metrics expose immediate failures, circuit-breaker state, pending counts, retry results, and `Failed` deliveries.
- Correlation identifiers connect the ticket operation, pending record, Function execution, and notification.
- Pending-delivery claims are atomic or otherwise safe under concurrent Function executions.
- Retry payloads contain only the minimum tenant and user data required.
- Manual review and replay of `Failed` deliveries must preserve idempotency.
- The Timer schedule can be changed without rebuilding the Function.
- A cleanup or retention policy for completed and failed delivery records must be defined before production.

## Reconsideration Triggers

Reconsider this decision if:

- Polling load or latency becomes unacceptable.
- Multiple consumers require the same events.
- Ordering or stronger delivery guarantees become mandatory.
- Pending-delivery volume materially increases.
- Operational maintenance exceeds the cost of a managed broker.
- The system adopts a broader event-driven architecture.

## Validation Criteria

1. Ticket Service commits before attempting notification delivery.
2. Notification failure cannot roll back the ticket.
3. Every request carries a unique operation identifier.
4. Retrying the same operation does not create duplicates.
5. Immediate calls use Polly timeout, exponential backoff, maximum attempts, and circuit breaker.
6. Failed immediate calls create durable pending-delivery records in Ticket Database.
7. Pending work survives container restarts.
8. The Timer-triggered Function initially polls every minute with configurable frequency.
9. The Function uses its own managed identity and least-privilege Ticket Database access.
10. Concurrent Function executions cannot deliver the same pending record simultaneously.
11. Exhausted deliveries become `Failed` and remain visible for manual review.
12. Azure Service Bus and other brokers are absent from the MVP.
13. Function-to-Notifications authentication remains visibly `TBD` until separately decided.
