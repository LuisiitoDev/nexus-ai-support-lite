# Nexus Support Lite

## Personas

**Status:** Discovery validated  
**Version:** 1.0  
**Date:** July 31, 2026  
**Related documents:** `PRODUCT.md`, `USER_FLOWS.md`

## 1. Purpose

This document describes the primary people expected to use Nexus Support Lite during its initial validation. It focuses on their real-world context, goals, frustrations, behaviors, and technology familiarity.

Roles, permissions, system behavior, and detailed journeys are defined in `PRODUCT.md` and `USER_FLOWS.md` and are not repeated here.

## 2. Validation Context

Nexus Support Lite will initially be validated with organizations from different industries that share one characteristic: they have an internal IT team responsible for handling support requests and incidents from their employees.

The primary personas are:

1. Requester.
2. Agent.
3. Organization Administrator.

These personas describe typical usage patterns rather than rigid job titles. The same person may hold more than one Nexus role, as defined in the existing product documentation.

## 3. Requester Persona

### Profile

The Requester is any employee in the organization who needs assistance from the internal IT team. They typically have basic technology familiarity and use Nexus primarily from a work computer.

Because they request help only occasionally, they may not remember the support process or be highly familiar with Nexus each time they use it.

### Current Behavior

When the Requester needs help, they commonly contact someone from IT directly through Microsoft Teams. This occurs because they do not know who the appropriate person is for their specific issue.

### Primary Goal

Use one clear place to request IT assistance and follow the incident after submitting it.

### Needs

- A clear and simple way to request assistance without knowing which IT person should handle the issue.
- Confidence that the request reached the appropriate team.
- Visibility into whether the incident is being handled.
- Visibility into the agent currently assigned to the incident.
- An understandable experience despite using the product only occasionally.

### Frustrations and Concerns

- Not knowing whom to contact when an IT issue occurs.
- Uncertainty about whether IT is already attending to the request.
- Depending on direct and informal communication through Microsoft Teams.

### Success from the Requester's Perspective

The Requester can submit an incident without identifying a specific IT contact, then clearly see that it is **In Progress** and which agent is handling it.

## 4. Agent Persona

### Profile

The Agent is an IT professional whose specialization depends on the topics assigned to them. An agent may work in technical support, software development, infrastructure, or another internal IT function.

Handling incidents is not necessarily their primary job. They commonly attend to them occasionally alongside other responsibilities.

### Current Behavior

The Agent receives support requests through multiple channels, mainly Microsoft Teams and email. These requests compete with their other responsibilities and do not provide one consistent place for tracking work.

### Primary Goal

Centralize support requests in one place so they can be handled without losing visibility or continuity.

### Needs

- A centralized source for incidents that currently arrive through Teams and email.
- Immediate visibility into incidents already assigned to them when entering Nexus.
- Enough information from the Requester to understand and work on the issue.
- A reliable record that helps preserve follow-up while they perform other duties.

### Frustrations and Concerns

- Receiving requests dispersed across different communication channels.
- Receiving incidents with insufficient information.
- Losing track of an incident among other work responsibilities.

### Success from the Agent's Perspective

The Agent enters Nexus, quickly identifies their assigned incidents, and can continue each case from a centralized record without reconstructing context from Teams or email.

## 5. Organization Administrator Persona

### Profile

The Organization Administrator is an operational user responsible for maintaining Nexus within the organization. They are not necessarily the head or coordinator of IT and typically have basic-to-intermediate technology familiarity.

They know the responsibilities and specialties of the organization's agents and use that knowledge when configuring topic responsibility.

### Current Responsibility

The Administrator keeps users, roles, and topics correctly configured for the organization.

### Primary Goal

Maintain an accurate operational configuration so requests can be routed through the appropriate topics to suitable agents.

### Needs

- A straightforward way to maintain users, roles, and topics.
- Clear visibility into which agents are responsible for each topic.
- Enough control to keep topic assignments aligned with agent responsibilities and specialties.
- An administration experience suitable for a basic-to-intermediate technology level.

### Frustrations and Concerns

- Determining which agents should be responsible for each topic, even though they understand the agents' specialties.
- Keeping the configuration aligned with how the IT team distributes its responsibilities.

### Success from the Administrator's Perspective

The Administrator can confidently maintain users, roles, topics, and responsible agents so every active topic has suitable coverage and incidents reach the appropriate team.

## 6. Cross-Persona Product Implications

The validated personas reinforce the following product considerations without changing the existing functional decisions:

- The interface should use clear language and remain easy to relearn for occasional users.
- The Requester experience should not assume knowledge of the IT team's structure.
- Assignment status and current ownership should be prominent because they provide confidence that an incident is being handled.
- The Agent home should prioritize already assigned incidents because agents handle support alongside other responsibilities.
- Incident records should preserve context well enough to reduce reliance on Teams and email.
- Administrative configuration should make topic coverage and responsible agents easy to understand.

These implications guide later design work. They do not add new scope or override the validated rules in `PRODUCT.md` and `USER_FLOWS.md`.

## 7. Discovery Boundaries

The following remain outside this persona document and should be addressed in later product discovery or validation:

- Persona-specific accessibility needs.
- Quantitative incident volume and usage frequency by organization.
- Detailed organizational size or IT team structure.
- Device and workflow variations beyond the primary desktop context validated for the Requester.
- Personas for the Nexus Global Administrator, because the Discovery completed for this document focused on tenant operational users.
- Evidence from interviews or usability testing with real users.

