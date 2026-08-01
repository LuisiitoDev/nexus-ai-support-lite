# ADR-001: Use Azure Container Apps for the MVP Runtime

**Status:** Accepted  
**Date:** August 1, 2026  
**Decision owners:** Nexus Support Lite team  
**Related documents:** `CONTAINER_DIAGRAM.md`, `DEPLOYMENT_DIAGRAM.md`, `DOMAIN_BOUNDARIES.md`

## Context

Nexus Support Lite requires an Azure runtime for independently deployable containers:

- Web Frontend.
- API Gateway.
- Identity Service.
- Ticket Service.
- Notification Service.

The MVP prioritizes low cost, low operational overhead, independent deployments, and the ability to grow without operating Kubernetes infrastructure prematurely. The expected initial traffic and number of services do not currently justify direct cluster administration.

The alternatives evaluated were Azure Container Apps and Azure Kubernetes Service (AKS).

## Decision

Use **Azure Container Apps** as the container execution platform for the Nexus Support Lite MVP.

Each deployable component will run as an independently deployable container app. Environment design, ingress exposure, scaling limits, revision strategy, health probes, networking, and workload profiles will be refined in later infrastructure decisions.

AKS is not selected for the MVP. It remains a future alternative if measurable scale, networking, portability, governance, or Kubernetes-specific requirements justify the additional operational responsibility.

## Decision Drivers

- Minimize infrastructure administration for the initial team.
- Support independent deployment and scaling of each container.
- Avoid maintaining Kubernetes nodes and cluster components before they are necessary.
- Support revision-based releases and controlled traffic changes.
- Allow low-traffic workloads to scale down where technically appropriate.
- Keep the architecture compatible with containerized services and a possible future migration.

## Consequences

### Positive

- The team can deploy containers without operating an AKS cluster.
- Each frontend or backend component can be released and scaled independently.
- The platform provides managed ingress, revisions, and autoscaling capabilities.
- Initial operational complexity is lower than managing Kubernetes directly.
- The runtime aligns with the MVP's low-traffic and cost-conscious posture.

### Negative and trade-offs

- The team has less control over the underlying orchestration platform than with AKS.
- Some Kubernetes-native tooling, controllers, and advanced scheduling patterns will not be directly available.
- Scaling to zero may introduce cold-start latency and must not be enabled blindly for latency-sensitive components.
- Networking, observability, persistent storage, quotas, and service limits must be validated against the final workload design.
- A future move to AKS would require deployment and operational changes even though the container images and service boundaries can remain largely portable.

## Alternatives Considered

### Azure Kubernetes Service

Rejected for the MVP because the current product does not require Kubernetes-level control, a large service estate, complex scheduling, or advanced cluster networking. AKS would introduce cluster operations, capacity planning, upgrades, security maintenance, and additional baseline infrastructure before those costs provide proportional value.

### Keep the runtime as TBD

Rejected because the architecture now has sufficient MVP constraints to make a responsible platform decision. Continuing with an unresolved runtime would block concrete deployment, networking, CI/CD, and cost design.

## Reconsideration Triggers

Re-evaluate this decision if one or more of the following become true:

- Container Apps limits prevent required networking, scaling, storage, or deployment behavior.
- The platform grows to a service estate whose governance is materially easier with Kubernetes.
- Workloads require Kubernetes-native operators, custom controllers, specialized scheduling, or privileged capabilities.
- Sustained workloads make an AKS cost model demonstrably more favorable.
- Regulatory or organizational requirements demand cluster-level controls unavailable in Container Apps.
- A portability requirement makes direct Kubernetes APIs and tooling a material product constraint.

## Validation Criteria

Before production launch, a deployment spike must demonstrate that:

1. The frontend, gateway, and three MVP services deploy independently.
2. Public ingress can be restricted to the intended entry points.
3. Service-to-service HTTP communication works within the selected environment topology.
4. Each service receives only its own secrets and database configuration.
5. Health probes, revision rollout, rollback, and scaling behavior are verified.
6. Expected monthly cost fits the approved MVP budget.
7. The selected region supports every required Container Apps capability.

