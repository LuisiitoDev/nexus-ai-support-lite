# Nexus Support Lite

## User Flows

**Status:** Architecture synchronized  
**Version:** 2.0  
**Date:** August 1, 2026  
**Related documents:** `PRODUCT.md`, `PERSONAS.md`, `SYSTEM_CONTEXT.md`, `CONTAINER_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`

## 1. Purpose

This document defines the validated journeys, system responses, permission boundaries, state transitions, notifications, and audit behavior for the Nexus Support Lite MVP.

## 2. Actors and Global Rules

- **Requester:** creates and follows their own incidents.
- **Agent:** handles incidents for topics where they are responsible.
- **Organization Administrator:** manages users, roles, topics, reports, and audit within one organization; incident access is read-only.
- **Nexus Global Administrator:** manages organization registration and identity configuration without tenant operational access.

A person may hold multiple roles. The active role controls navigation and actions.

Global invariants:

- Data and actions are isolated by organization.
- Tenant context comes exclusively from the validated Entra `tid` claim.
- Lifecycle is **New → In Progress → Closed**.
- New incidents have a topic and no assignee.
- In Progress incidents have exactly one assignee.
- Closed incidents have a required resolution.
- Topic, role, assignment, account state, and tenant permissions are evaluated for every operation.
- Relevant actions record actor or system origin, time, and changed values.
- Comments are visible to the requester; internal notes are outside the MVP.

## 3. Access and Active Role

### 3.1 Organizational Sign-in

1. The user selects Microsoft organizational sign-in.
2. The frontend redirects to Microsoft Entra ID through the shared multitenant frontend App Registration.
3. After authentication, the frontend requests an access token for the shared API App Registration.
4. The frontend sends the token to the API Gateway without a separately supplied tenant.
5. The Gateway validates signature, issuer, audience, and expiration.
6. Nexus resolves the organization exclusively from `tid`.
7. If the tenant is unknown or disabled, Nexus denies access without exposing another organization's information.
8. Identity resolves the local user, account state, and Nexus roles.
9. On first access, Identity creates the local user with the **Requester** role from validated token claims.
10. Nexus opens the interface for the active role.

### 3.2 Choosing and Changing Roles

- A single role activates directly.
- A user with several roles may change the active role.
- Each role exposes only permitted navigation, actions, and data.

### 3.3 Changes During an Active Session

Role and account changes apply immediately through Gateway cache invalidation.

If the active role is removed, Nexus selects the next available role in this order:

1. Organization Administrator
2. Agent
3. Requester

No selection screen is shown for this automatic transition. If all roles are removed or the account is deactivated, Nexus closes active sessions immediately.

## 4. Requester Flows

### 4.1 Home and Incident Detail

The requester sees only their incidents and may distinguish them by status, priority, topic, and assignee. Detail includes number, subject, description, status, priority, topic, **Assigned to**, comments, permitted attachments, visible history, and closure information.

### 4.2 Create Incident

1. Select **Create incident**.
2. Enter subject and description.
3. Select an active topic and a priority: Low, Medium, High, or Critical.
4. Optionally add permitted attachments.
5. Submit.
6. Nexus validates fields, topic status, attachment constraints, authorization, and trusted tenant context.
7. Nexus creates the incident as **New** and unassigned.
8. It appears in the shared queue of responsible agents.
9. Those agents receive an in-app notification.
10. Nexus opens the incident or presents a success result from which it can be opened.

Knowledge Base and AI suggestions are not part of this flow. Topic and priority are selected manually.

An inactive or invalid topic blocks submission and requires another active topic.

### 4.3 Comment or Attachment

1. Open one of the requester's incidents.
2. Add a comment and optional permitted attachments.
3. Nexus records author and time.
4. If assigned, the current agent is notified.
5. If New and unassigned, topic agents are not notified about the comment.

### 4.4 Requester Notifications

Notify the requester when an agent comments, changes priority, transfers the incident, or closes it.

## 5. Agent Flows

### 5.1 Home

- **New:** unassigned incidents in the agent's responsible topics.
- **My incidents:** incidents assigned to the agent and In Progress.

There is no In Progress + Unassigned queue because that state is invalid.

### 5.2 Take Incident

1. Select **Take incident** directly from New.
2. Nexus asks for confirmation.
3. Cancel leaves everything unchanged.
4. Confirm performs an atomic assignment.
5. Success changes New to In Progress and assigns the agent.
6. Nexus immediately opens the detail.
7. Reviewing or changing priority is optional.

If another agent won the race, Nexus rejects the stale request, explains that it is assigned, refreshes the list, and removes it from availability. Other topic agents are not notified when an incident is taken.

### 5.3 Work Assigned Incident

The assignee may comment, attach permitted files, change priority, delegate, transfer topic, and close. Prior history and contributions remain visible.

### 5.4 Agent Comment

Nexus records the visible comment and optional attachment with actor and time, then notifies the requester.

### 5.5 Change Priority

Nexus applies the selected priority without mandatory justification, records old/new values and time, and notifies the requester.

### 5.6 Delegate

1. The assignee selects **Delegate**.
2. Nexus lists only active responsible agents of the current topic.
3. The assignee chooses a recipient and confirms.
4. Delegation is immediate; acceptance is not required.
5. The incident remains In Progress with the new assignee.
6. Nexus records both assignees, actor, and time.
7. The new assignee is notified.
8. The previous assignee returns to Agent Home.

### 5.7 Transfer Topic

1. The assignee selects **Transfer**.
2. Select an active destination topic.
3. Enter mandatory justification.
4. Confirm.
5. Nexus changes topic, removes the assignee, and returns the incident to New.
6. It appears in the destination queue.
7. Destination agents are not notified.
8. The requester is notified.
9. Justification and transfer details are visible to the requester and retained in history.
10. The former assignee returns home.

History includes previous/new topic, prior assignee, justification, actor, and time.

### 5.8 Close Incident

1. The assignee selects **Close incident**.
2. Enter required resolution.
3. Submit and review final confirmation.
4. Cancel preserves drafted text and keeps the incident In Progress.
5. Confirm changes it to Closed, stores resolution and closure time, and preserves history.
6. The requester is notified.
7. The agent returns home.

Only the current assignee may close.

### 5.9 Agent Removed from Topic

When an assigned agent loses responsibility for the topic:

1. Nexus removes assignment.
2. In Progress returns to New.
3. The incident returns to the shared queue.
4. Current responsible agents are notified.
5. Existing comments, attachments, progress, and draft resolution are preserved.
6. History records the former agent, state change, reason, and time.

## 6. Organization Administrator Flows

### 6.1 Dashboard and Incident Consultation

The administrator sees tenant-scoped summaries and may open filtered lists. They have read-only access to incident detail, complete history, comments, attachments, resolution, and closure data.

They cannot comment, assign, delegate, transfer, change priority, or close.

### 6.2 Manage Users

Administrators can view users, assign/remove roles, assign/remove agent topics, deactivate/reactivate accounts, and inspect user audit. Users are deactivated, not permanently deleted.

#### Change roles or topics

1. Edit roles or topic responsibility.
2. Nexus ensures no active topic loses its last responsible agent.
3. Valid changes apply immediately and invalidate relevant Gateway cache entries.
4. Audit records administrator, time, and old/new values.
5. No reason is required.
6. Active-session behavior follows section 3.3.

#### Deactivate user

1. Select **Deactivate user** and provide a required reason.
2. Confirm.
3. Nexus disables the account, invalidates access state, and closes sessions.
4. Assigned incidents return to New and unassigned.
5. They return to normal topic queues without notifying topic agents.
6. Prior work remains.
7. Incident history records former agent, state change, reason, and time.
8. User audit records administrator and mandatory reason.

#### Reactivate user

1. Select **Reactivate user** and provide a required reason.
2. Nexus restores prior roles and topic responsibility subject to current validation.
3. Released incidents remain New and are not automatically reassigned.
4. Audit records administrator, reason, and time.

### 6.3 Manage Topics

Topics are never permanently deleted.

- Creation requires name and description.
- Activation requires at least one responsible agent.
- Editing name/description preserves audit.
- Removing a responsible agent is blocked if it would leave an active topic without agents.
- Assigned incidents affected by removal follow section 5.9.
- Deactivation is blocked while open incidents remain.
- Once no open incidents remain, deactivation preserves historical associations.

### 6.4 User Audit

Audit includes activation/deactivation, role changes, topic changes, responsible administrator, time, required reasons, and previous/new values.

## 7. Notification Flows

### 7.1 Events

| Event | Recipient | Rule |
| --- | --- | --- |
| Incident created | Responsible topic agents | Notify |
| Agent takes incident | Other topic agents | Do not notify |
| Agent comments | Requester | Notify |
| Requester comments on assigned incident | Assignee | Notify |
| Requester comments on New incident | Topic agents | Do not notify |
| Agent changes priority | Requester | Notify |
| Delegation | New assignee | Notify |
| Topic transfer | Requester | Notify |
| Topic transfer | Destination agents | Do not notify |
| Incident closed | Requester | Notify |
| Assigned agent removed from topic | Current topic agents | Notify |
| Deactivated agent's incidents released | Topic agents | Do not notify |

### 7.2 Bell and History

- Mark one or all notifications read.
- Mark a read notification unread; it re-enters the unread count.
- Opening a notification marks it read.
- Selecting an incident notification opens the incident when authorized.
- Notifications remain visible in the bell for 60 days.
- After 60 days they leave the bell but are not deleted.
- History contains all notifications, newest first, 10 records per page.
- History filters: date range, read state, and event type.

### 7.3 Permission Loss

Notifications are created only for users authorized at creation time. If access is later lost, Nexus displays **Access unavailable** and exposes no incident data; the notification remains according to normal history rules.

### 7.4 Delivery Failure

1. Tickets commits the business change before requesting notification creation.
2. The request includes a unique operation identifier.
3. Notifications treats retries idempotently.
4. Immediate delivery uses Polly timeout, exponential-backoff retries, maximum attempts, and circuit breaker.
5. Failure never reverses the ticket change.
6. Tickets stores a durable pending delivery after immediate attempts fail.
7. The Timer-triggered Azure Function retries eligible deliveries.
8. Exhausted deliveries become Failed for manual review.

A delayed notification does not change the already completed user action.

## 8. State Transitions

| Trigger | Previous | New | Assignment |
| --- | --- | --- | --- |
| Create | — | New | Unassigned |
| Take | New | In Progress | Taking agent |
| Delegate | In Progress | In Progress | Selected current-topic agent |
| Transfer | In Progress | New | Unassigned |
| Assignee removed from topic | In Progress | New | Unassigned |
| Assignee deactivated | In Progress | New | Unassigned |
| Close | In Progress | Closed | Closing agent retained in history |

No supported transition produces In Progress + Unassigned.

## 9. Validation and Error Behavior

- **Unknown/disabled Entra tenant:** deny access without leaking organization information.
- **Disabled account or no roles:** deny access and close active sessions when applicable.
- **Concurrent take:** reject loser, explain, and refresh.
- **Missing closure resolution:** block closure and identify field.
- **Missing transfer justification:** block transfer and identify field.
- **Inactive topic:** block creation/transfer and require active topic.
- **Invalid delegation recipient:** block unless active and responsible for current topic.
- **Last responsible agent removal:** block while topic is active.
- **Topic deactivation with open incidents:** block until transfer.
- **Unauthorized notification target:** show Access unavailable without incident content.
- **Notification outage:** preserve business result and enqueue durable retry.

## 10. Audit Requirements

Incident history records creation, state and priority changes, assignment, delegation, transfer and justification, comments, attachments, automatic release, closure/resolution, actor or system origin, time, and old/new values.

Historical data remains visible to authorized users after users or topics are deactivated.

## 11. Acceptance Criteria

1. Access uses Entra organizational authentication and resolves the organization only from validated `tid`.
2. First access creates a Requester under an enabled organization.
3. Requesters manually select topic and priority; AI is not required or present.
4. New incidents route only to responsible agents.
5. Exactly one concurrent take succeeds and opens the detail.
6. Taking does not force priority confirmation.
7. Every In Progress incident has one assignee.
8. Delegation is immediate and limited to current-topic agents.
9. Transfer requires justification, returns to New, and notifies requester but not destination agents.
10. Closure requires resolution and confirmation; cancel preserves draft text.
11. Topic-responsibility removal releases incidents and preserves work.
12. Deactivation closes sessions and releases incidents without notifying topic agents.
13. Reactivation does not restore old assignments.
14. Active topics always have at least one responsible agent.
15. Topics with open incidents cannot be deactivated.
16. Organization administrators have read-only incident access.
17. Bell read state, unread count, 60-day visibility, and permanent history work as defined.
18. Notification delivery is idempotent and recoverable without rolling back Tickets.
19. All access remains isolated by tenant.
20. Knowledge Base, AI, Service Bus, and other brokers are absent from the MVP.
