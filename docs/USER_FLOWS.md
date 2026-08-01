# Nexus Support Lite

## User Flows

**Status:** Discovery validated  
**Version:** 1.0  
**Date:** July 31, 2026  
**Related specification:** `PRODUCT.md`

## 1. Purpose and Decision Precedence

This document defines the validated user journeys, system responses, alternative paths, permission boundaries, and audit behavior for Nexus Support Lite.

Where a rule in this document differs from an earlier statement in `PRODUCT.md`, the rule captured here represents the latest validated Discovery decision and takes precedence for user-flow design. The product specification should be synchronized before implementation.

## 2. Actors

- **Requester:** creates and follows their own incidents.
- **Agent:** handles incidents belonging to topics for which they are responsible.
- **Organization Administrator:** manages users, roles, topics, reporting, and audit data within one organization; incident access is read-only.
- **Nexus Global Administrator:** manages organizations and identity configuration without access to tenant operational data.

A person may have more than one role. The active role determines the interface and available actions.

## 3. Global Rules

- All data and actions are isolated by organization.
- The supported incident lifecycle is **New → In Progress → Closed**.
- A **New** incident has a topic and no individual assignee.
- An **In Progress** incident must always have an assignee. Nexus must never retain an incident as **In Progress and Unassigned**.
- A **Closed** incident contains a required resolution.
- Topic, role, assignment, and tenant permissions are evaluated for every operation.
- Relevant actions are recorded with actor, date, and time.
- All incident comments are visible to the requester; internal notes are not included in the first version.

## 4. Access and Active Role

### 4.1 Sign In

1. The person enters their email address.
2. Nexus resolves the organizations associated with the email and their identity providers.
3. If only one organization is available, Nexus continues with it.
4. If several organizations are available, Nexus asks the person to select one.
5. Nexus redirects the person to the selected organization's identity provider.
6. After successful authentication, Nexus creates a first-time account with the **Requester** role when needed.
7. Nexus opens the interface for the active role.

### 4.2 Choosing and Changing Roles

1. If the person has one role, Nexus activates it directly.
2. If the person has several roles, they may change the active role from the application.
3. Each role displays only its permitted navigation and data.

### 4.3 Role Changes During an Active Session

- Role and topic changes apply immediately, even during an active session.
- If the active role is removed, Nexus automatically selects the next available role using this fixed hierarchy:
  1. **Organization Administrator**
  2. **Agent**
  3. **Requester**
- Nexus does not show the role-selection screen for this automatic transition.
- If all roles are removed, Nexus immediately closes all active sessions for the user.
- If an account is deactivated, Nexus immediately closes all of that user's active sessions.

## 5. Requester Flows

### 5.1 Requester Home

The requester can view their incidents and distinguish them by status, priority, topic, and current assignee. Available filters follow the product specification and the user's tenant permissions.

### 5.2 Create an Incident

1. The requester selects **Create incident**.
2. Nexus asks for a subject and free-text description.
3. The requester may add permitted attachments.
4. AI analyzes the subject and description and suggests a topic and priority.
5. Nexus clearly presents both values as suggestions.
6. The requester may accept or freely change either suggestion without justification.
7. The requester submits the incident.
8. Nexus validates the required fields, active topic, attachment constraints, and tenant context.
9. Nexus creates the incident as **New** and **Unassigned**.
10. The incident appears in the shared queue of agents responsible for the selected topic.
11. All agents responsible for that topic receive a notification in the Nexus bell.
12. Nexus opens the created incident or presents a successful creation result from which it can be opened.

#### AI unavailable

1. Nexus informs the requester that suggestions are unavailable.
2. The requester selects the topic and priority manually.
3. Incident creation continues without interruption.

#### Invalid or inactive topic

- Nexus prevents submission and asks the requester to choose an active topic.

### 5.3 View an Incident

The requester can view:

- Incident number, subject, and description.
- Status, priority, topic, and **Assigned to**.
- Chronological comments.
- Permitted attachments.
- Visible action history.
- Recorded resolution and closure information when closed.

The requester cannot view incidents created by another requester unless a future permission explicitly grants it.

### 5.4 Add a Comment or Attachment

1. The requester opens one of their incidents.
2. The requester writes a comment and may add permitted attachments.
3. Nexus validates and records the contribution with author, date, and time.
4. If the incident is assigned, the assigned agent receives a notification.
5. If the incident is **New** and unassigned, Nexus does not notify all responsible agents about the comment.

### 5.5 Requester Notifications

The requester receives a notification when:

- An agent adds a comment.
- An agent changes the incident priority.
- The incident is transferred to another topic.
- The incident is closed.

## 6. Agent Flows

### 6.1 Agent Home

The agent's main screen contains:

- **New:** unassigned incidents belonging to the agent's responsible topics.
- **My incidents:** incidents currently assigned to the agent and therefore **In Progress**.

There is no separate **Unassigned** section for incidents in progress because that state combination is invalid.

### 6.2 Take a New Incident

1. From **New**, the agent selects **Take incident** directly without first opening its detail.
2. Nexus displays a confirmation prompt.
3. If the agent cancels, nothing changes.
4. If the agent confirms, Nexus performs an atomic assignment operation.
5. On success, Nexus assigns the incident to the agent and changes its status from **New** to **In Progress**.
6. Nexus immediately opens the incident detail.
7. The agent may begin working immediately.
8. Reviewing or changing priority is optional; the agent does so only when considered necessary.

#### Another agent took it first

1. Nexus rejects the stale assignment attempt.
2. Nexus informs the agent that the incident has already been assigned.
3. Nexus automatically refreshes the list.
4. The incident no longer appears as available.

Taking an incident does not notify the other agents responsible for its topic.

### 6.3 Work an Assigned Incident

The assigned agent can:

- Add visible comments.
- Add permitted attachments.
- Change priority.
- Delegate the incident.
- Transfer the incident to another topic.
- Close the incident.

The agent can review all prior history, comments, attachments, and work recorded by previous assignees before continuing.

### 6.4 Add an Agent Comment

1. The assigned agent adds a comment and optional permitted attachments.
2. Nexus records the author, date, and time.
3. The comment is visible to the requester.
4. The requester receives a notification.

### 6.5 Change Priority

1. The assigned agent selects another priority.
2. Nexus applies the change without requiring a justification.
3. Nexus records the old and new priorities, actor, date, and time.
4. The requester receives a notification.

### 6.6 Delegate an Incident

1. The current assignee selects **Delegate**.
2. Nexus lists only active agents responsible for the incident's current topic.
3. The agent selects the new assignee and confirms the action.
4. Delegation takes effect immediately; acceptance by the recipient is not required.
5. The incident remains **In Progress** and changes assignee.
6. Nexus records the previous assignee, new assignee, actor, date, and time.
7. The new assignee receives a notification.
8. Nexus returns the previous assignee to the agent home screen.

### 6.7 Transfer an Incident to Another Topic

1. The assigned agent selects **Transfer**.
2. Nexus asks for an active destination topic.
3. The agent enters a mandatory justification.
4. Nexus displays a confirmation.
5. On confirmation, Nexus changes the topic, removes the assignee, and changes the status to **New**.
6. The incident appears in the shared queue for the destination topic.
7. Nexus does **not** notify the agents responsible for the destination topic.
8. Nexus notifies the requester that the incident was transferred.
9. The justification and transfer details are visible to the requester and retained in history.
10. Nexus returns the previous assignee to the agent home screen.

The history records the previous topic, new topic, previous assignee, justification, actor, date, and time.

### 6.8 Close an Incident

1. The assigned agent selects **Close incident**.
2. Nexus requires a resolution description.
3. The agent submits the closure.
4. Nexus displays a final confirmation.
5. If the agent cancels, Nexus preserves the entered resolution text and keeps the incident **In Progress**.
6. If the agent confirms, Nexus changes the status to **Closed**, records the resolution and closure timestamp, and preserves the complete history.
7. The requester receives a notification.
8. Nexus returns the agent to the main screen.

Only the current assignee can close an incident.

### 6.9 Agent Loses Responsibility for the Topic

If an assigned agent is removed from the incident's topic:

1. Nexus automatically removes the individual assignment.
2. Nexus changes the incident from **In Progress** to **New**.
3. The incident returns to the shared queue of the topic's current responsible agents.
4. Those agents receive a notification in the bell.
5. Nexus preserves all comments, attachments, progress, and partial resolution text.
6. The history records the prior agent, date and time, status change, and automatic unassignment reason.

## 7. Organization Administrator Flows

### 7.1 Administrator Dashboard

1. Nexus displays a general incident summary by status, priority, and topic.
2. Selecting an indicator opens the incident list with the corresponding filter already applied.
3. Reports and incident data remain limited to the active organization.

### 7.2 Consult Incidents

The organization administrator has read-only access to all incidents in the organization and may view:

- Incident details.
- Complete history.
- Comments.
- Attachments.
- Resolution and closure data.

The administrator cannot comment, reassign, delegate, transfer, change priority, or close an incident.

### 7.3 Manage Users

The administrator can:

- View users in the organization.
- Assign and remove roles.
- Assign and remove the topics handled by agents.
- Deactivate and reactivate users.
- Consult the user's audit history.

Users are never permanently deleted; they are deactivated to preserve history and incident participation.

#### Change roles or topics

1. The administrator edits the roles or topic responsibilities.
2. Nexus validates that no active topic will be left without a responsible agent.
3. Nexus applies valid changes immediately, including during active user sessions.
4. Nexus automatically records the administrator, date and time, and previous and new values.
5. A reason is not required.
6. Active-session behavior follows section 4.3.

#### Deactivate a user

1. The administrator selects **Deactivate user**.
2. Nexus requires a deactivation reason.
3. After confirmation, Nexus deactivates the account and immediately closes all active sessions.
4. If the user was an agent with assigned incidents, each incident returns to **New** and becomes **Unassigned**.
5. Those incidents appear in their topics' normal shared queues; responsible agents are not notified.
6. Nexus preserves every incident's existing work and history.
7. Each affected incident records the previous agent, timestamp, change from **In Progress** to **New**, and deactivation as the reason.
8. The user audit records the administrator, date and time, and mandatory reason.

#### Reactivate a user

1. The administrator selects **Reactivate user**.
2. Nexus requires a reactivation reason.
3. Nexus restores the user's previous roles and topic assignments, subject to current topic validation rules.
4. Incidents released during deactivation remain **New** and **Unassigned**; Nexus does not return them automatically to the reactivated agent.
5. The user audit records the administrator, date and time, and mandatory reason.

### 7.4 Manage Topics

The administrator can create, edit, activate, and deactivate topics. Topics are never permanently deleted.

#### Create and activate a topic

1. The administrator enters the topic name and description.
2. At least one responsible agent must be assigned before activation.
3. Nexus prevents activation until that condition is met.

#### Edit a topic

1. The administrator may modify the name and description even when incidents are associated with the topic.
2. Nexus records the administrator, date and time, and old and new values.
3. The administrator can consult this audit history from the topic detail.

#### Remove a responsible agent

1. Nexus verifies how many active responsible agents remain.
2. If the change would leave an active topic without any responsible agent, Nexus blocks it until another agent is assigned.
3. If the removed agent has assigned incidents in that topic, each affected incident follows section 6.9.

#### Deactivate a topic

1. Nexus verifies whether the topic has open incidents.
2. If open incidents exist, Nexus blocks deactivation until they are transferred to another active topic.
3. When no open incidents remain, the administrator may deactivate the topic.
4. The inactive topic remains associated with historical incidents.

### 7.5 User Audit Detail

The administrator can consult a complete history of:

- Activations and deactivations.
- Role changes.
- Topic-assignment changes.
- Responsible administrator.
- Date and time.
- Reasons when required.
- Previous and new values.

## 8. Notification Flows

### 8.1 Notification Events

| Event | Recipient | Notification rule |
| --- | --- | --- |
| Incident created | All agents responsible for the topic | Notify |
| Agent takes incident | Other responsible agents | Do not notify |
| Agent comments | Requester | Notify |
| Requester comments on assigned incident | Assigned agent | Notify |
| Requester comments on new unassigned incident | Topic agents | Do not notify |
| Agent changes priority | Requester | Notify |
| Incident delegated | New assignee | Notify |
| Incident transferred | Requester | Notify |
| Incident transferred | Agents of destination topic | Do not notify |
| Incident closed | Requester | Notify |
| Assigned agent removed from topic | Current responsible agents | Notify |
| Agent account deactivated and incidents released | Topic agents | Do not notify |

### 8.2 Notification Bell

- A notification can be marked individually as read.
- The user can select **Mark all as read**.
- A read notification can be marked as unread.
- Marking it unread adds it back to the bell's unread counter.
- Opening a notification automatically marks it as read.
- Selecting an incident notification opens that incident's detail.
- Notifications remain visible in the bell for 60 days.
- After 60 days, they stop appearing in the bell but are not deleted.

### 8.3 Permission Loss

Notifications should only be created for users who can access the referenced incident. If a user later loses access:

1. Nexus does not expose incident data.
2. Nexus displays **Access unavailable**.
3. The notification remains visible according to the normal retention and display rules.

### 8.4 Notification History

1. The user opens **Notification history**.
2. Nexus displays all notifications, including those older than 60 days.
3. Results are ordered from newest to oldest.
4. The screen displays 10 records per page.
5. The user can filter by:
   - Date range.
   - Read status.
   - Event type.
6. Read and unread actions remain available according to the notification rules.

## 9. Incident History and Audit Requirements

The incident history is chronological and must record, where applicable:

- Incident creation.
- Status changes.
- Priority changes.
- Assignment and automatic assignment failure outcomes where relevant.
- Delegations.
- Topic transfers and mandatory justifications.
- Comments and attachments.
- Automatic release caused by loss of topic responsibility.
- Automatic release caused by user deactivation.
- Closure and resolution.
- Actor or system origin.
- Date and time.
- Previous and new values when applicable.

Historical information remains accessible to authorized users even when an agent, user, or topic is later deactivated.

## 10. Key State Transitions

| Trigger | Previous state | New state | Assignment result |
| --- | --- | --- | --- |
| Incident created | — | New | Unassigned |
| Agent takes incident | New | In Progress | Taking agent |
| Agent delegates | In Progress | In Progress | Selected agent from current topic |
| Agent transfers topic | In Progress | New | Unassigned |
| Assigned agent removed from topic | In Progress | New | Unassigned |
| Assigned agent deactivated | In Progress | New | Unassigned |
| Assigned agent closes incident | In Progress | Closed | Closing agent retained in history |

No supported transition produces **In Progress + Unassigned**.

## 11. Validation and Error Behavior

- **Concurrent take:** reject the losing request, explain that the incident was already assigned, and refresh the queue.
- **Missing closure resolution:** prevent closure and identify the required field.
- **Missing transfer justification:** prevent transfer and identify the required field.
- **Inactive destination topic:** prevent creation or transfer and request an active topic.
- **Invalid delegation recipient:** prevent delegation unless the recipient is an active responsible agent for the current topic.
- **Last responsible agent removal:** block the change while the topic is active.
- **Topic deactivation with open incidents:** block until all open incidents are transferred.
- **Unauthorized notification target:** display **Access unavailable** without exposing incident content.
- **AI unavailable:** continue with manual topic and priority selection.

## 12. Flow-Level Acceptance Criteria

1. A requester can create an incident even when AI suggestions are unavailable.
2. A new incident is routed only to its active topic's responsible agents.
3. An agent can take an incident directly from the **New** list after confirmation.
4. Only one concurrent take succeeds, and the losing user's queue refreshes.
5. Taking an incident opens its detail and does not require a priority action.
6. Every **In Progress** incident has exactly one current assignee.
7. Delegation is immediate and limited to responsible agents of the current topic.
8. Topic transfer requires justification, returns the incident to **New**, and leaves it unassigned.
9. Destination-topic agents are not notified of a transfer; the requester is notified.
10. Closure requires a resolution and final confirmation; canceling preserves the drafted resolution.
11. Removing an assigned agent from a topic returns affected incidents to **New** and preserves all prior work.
12. Deactivating a user immediately closes their sessions and releases assigned incidents without notifying topic agents.
13. Reactivating a user restores roles and topics but does not restore former incident assignments.
14. An active topic always has at least one responsible agent.
15. A topic with open incidents cannot be deactivated.
16. Organization administrators can inspect incidents but cannot perform incident-handling actions.
17. Notification read state, unread count, 60-day bell visibility, and permanent history behavior work as defined.
18. All user, topic, incident, notification, and audit access remains isolated by tenant.

## 13. Required Synchronization with `PRODUCT.md`

Before implementation, update the product specification to reflect these validated decisions:

- Taking an incident opens its detail and does not require mandatory priority confirmation.
- Delegation is limited to responsible agents of the current topic and takes effect immediately.
- Topic transfer requires a visible mandatory justification.
- Topic transfer notifies the requester but does not notify agents of the destination topic.
- **In Progress + Unassigned** is invalid; automatic unassignment returns an incident to **New**.
- The organization administrator's incident access is strictly read-only.
- User, topic, audit, active-session, and notification-history rules defined in this document.

