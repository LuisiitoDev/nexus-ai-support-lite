# ADR-006: Testing Strategy and Test Project Structure

- **Status:** Accepted
- **Date:** August 2, 2026
- **Decision owners:** Nexus Support Lite architecture
- **Related:** `DOMAIN_BOUNDARIES.md`, `ADR-002-MULTITENANT-IDENTITY.md`, `ADR-003-PERSISTENCE-STRATEGY.md`

## Context

The repository had an empty `tests/` folder and no test project wired into `NexusAiSupportLite.slnx`. The first Infrastructure code landed for Identity (`InfrastructureDependencyExtensions`, `IdentityDatabaseHealthCheck`) with no automated coverage. A testing approach needs to be fixed now, before more services accumulate untested Infrastructure and Application code.

Licensing is a constraint: dependencies must stay MIT-licensed (or comparably permissive). FluentAssertions moved to a commercial license (Xceed, BUSL-style terms) starting with v8, so it is excluded despite being a common default.

## Decision

### Framework and libraries

- **xUnit** as the test runner (MIT).
- **Moq** for mocking (MIT), not NSubstitute.
- **Plain `Assert.*`** from xUnit for assertions — no FluentAssertions or other assertion library, to avoid non-MIT dependencies and an extra abstraction over a built-in API.
- `Microsoft.NET.Test.Sdk` and `xunit.runner.visualstudio` for test execution, matching the target framework (`net10.0`) used across `src/`.

### Project structure

Test projects mirror the `src/` domain-boundary layout, one test project per `src` project under test:

```
tests/Services/{Domain}/{ProjectUnderTest}.Tests/
```

For example, `src/Services/Identity/NexusSupport.Identity.Infrastructure` is covered by `tests/Services/Identity/NexusSupport.Identity.Infrastructure.Tests`. This keeps ownership boundaries between Identity, Ticket, and KnowledgeBase intact in tests the same way `DOMAIN_BOUNDARIES.md` keeps them intact in `src/`, and avoids a single monolithic test project that would blur those boundaries.

Each test project is added to `NexusAiSupportLite.slnx` under a matching `/tests/Services/{Domain}/` solution folder.

### Mocking EF Core `DbContext`

`DbContext.Database` and `DatabaseFacade`'s async methods (e.g. `CanConnectAsync`) are `virtual`, so Infrastructure code that depends on `DbContext` is unit-testable with Moq directly — mock the concrete `DbContext` subclass and its `Database` facade — without standing up a real or in-memory database. This was chosen over the EF Core InMemory provider to keep health-check-style tests fast, dependency-free, and focused on behavior rather than provider semantics.

### Scope of this decision

This ADR fixes the framework, mocking approach, and project layout. It does not yet mandate coverage thresholds, integration/E2E test infrastructure (e.g. Testcontainers for SQL Server), or a CI gate — those are left for a follow-up decision once more services have Application/Infrastructure logic worth testing that way.

## Consequences

### Positive

- A concrete, repeatable pattern exists for adding tests to any service without re-deciding tooling each time.
- Test project layout makes it obvious which `src` project lacks coverage.
- Dependency set stays MIT-licensed end to end.
- DbContext mocking keeps unit tests fast and independent of a real database or Docker.

### Negative

- Mocking `DbContext`/`DatabaseFacade` directly is more verbose than using the EF Core InMemory provider, and only works because the relevant members happen to be `virtual`; it will not extend to code that relies on LINQ query translation.
- No integration test project or CI test gate exists yet — this ADR does not by itself prevent regressions from shipping untested.
- One test project per `src` project increases the number of `.csproj` files to maintain as services grow.

## Alternatives Considered

### FluentAssertions for assertions

Rejected due to its commercial license as of v8, which conflicts with the MIT-only dependency constraint.

### NSubstitute for mocking

Rejected in favor of Moq per explicit preference; both are MIT-licensed, so the choice is stylistic rather than a licensing necessity.

### EF Core InMemory provider for DbContext-dependent tests

Rejected for the initial health-check tests in favor of mocking `DatabaseFacade.CanConnectAsync` directly, since the InMemory provider does not faithfully represent connectivity checks and would add a package dependency for a single boolean behavior. Remains a valid option for future tests that exercise actual query/persistence logic.

### Single shared test project for all services

Rejected because it would blur the same domain boundaries `DOMAIN_BOUNDARIES.md` establishes for `src/`, and would force unrelated services to share a `.csproj` and package version set.

## Reconsideration Triggers

Reconsider this decision if:

- Application-layer logic (not just Infrastructure) needs coverage and a different mocking shape is required.
- Integration tests against a real SQL Server (e.g. via Testcontainers) become necessary.
- A CI pipeline is introduced and needs a coverage gate or reporting format this ADR doesn't specify.
- The MIT-only constraint changes.

## Validation Criteria

1. Test projects live under `tests/Services/{Domain}/{ProjectUnderTest}.Tests`, mirroring `src/`.
2. All test dependencies (xUnit, Moq, Microsoft.NET.Test.Sdk, xunit.runner.visualstudio) are MIT-licensed.
3. No FluentAssertions or other non-MIT assertion library is referenced.
4. Each test project is registered in `NexusAiSupportLite.slnx` under a matching solution folder.
5. `dotnet test` runs and passes for each test project without requiring a live database.
