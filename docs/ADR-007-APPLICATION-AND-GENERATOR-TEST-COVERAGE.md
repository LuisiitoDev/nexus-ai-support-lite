# ADR-007: Extending Test Coverage to the Application Layer and the Source Generator

- **Status:** Accepted
- **Date:** August 2, 2026
- **Decision owners:** Nexus Support Lite architecture
- **Related:** `ADR-006-TESTING-STRATEGY.md`

## Context

`ADR-006-TESTING-STRATEGY.md` fixed the test framework (xUnit + Moq, plain `Assert.*`, no FluentAssertions) and the project layout (`tests/{mirroring-src}/...Tests`) for the Infrastructure layer, and explicitly flagged two reconsideration triggers:

- "Application-layer logic (not just Infrastructure) needs coverage and a different mocking shape is required."
- Coverage for code that doesn't fit the DbContext-mocking shape ADR-006 was written around.

Both triggers are now active:

1. `NexusSupport.Identity.Application` (added in a prior change) wraps each Identity Domain repository in a service that maps Domain models to DTOs. It has no coverage.
2. `NexusSupport.SourceGenerator` (the Roslyn incremental generator that emits `AddGeneratedServices()` DI registration extensions from `[Service(...)]`-annotated classes) has no coverage either, and it does not fit ADR-006's mocking shape at all: its core logic (`ServiceRegistrationExtractor`, `ServiceRegistrationSourceBuilder`) has no injectable dependency to substitute with Moq. Its actual behavior is "given C# source text, what source/diagnostics does the generator emit," which only Roslyn's own compilation APIs can exercise.

## Decision

### Identity Application layer: extend ADR-006's existing pattern, no new decision needed

`NexusSupport.Identity.Application.Tests` (`tests/Services/Identity/NexusSupport.Identity.Application.Tests`) follows ADR-006 as-is: xUnit + Moq, one test class per service, mocking the Domain repository interface the service under test depends on (the same shape already used for `IdentityDatabaseHealthCheckTests`, just substituting a repository interface for a mocked `DbContext`). No new library or pattern was required — the "different mocking shape" ADR-006 anticipated turned out to be the same shape, just against a plain interface instead of a `DbContext` subclass.

### Source generator: black-box test through the public `IIncrementalGenerator` API

`NexusSupport.SourceGenerator.Tests` (`tests/Shared/NexusSupport.SourceGenerator.Tests`, under a new `/tests/Shared/` solution folder mirroring `/src/Shared/`) tests `ServiceRegistrationGenerator` end-to-end rather than unit-testing its `internal` `ServiceRegistrationExtractor`/`ServiceRegistrationSourceBuilder` classes in isolation:

- A test builds an in-memory `CSharpCompilation` from a synthetic source string containing `[Service(...)]`-annotated classes.
- It runs that compilation through `CSharpGeneratorDriver.Create(new ServiceRegistrationGenerator())` and inspects the result via `GeneratorDriverRunResult` (`GeneratedSources` — hint names and emitted text) and the diagnostics collection returned by `RunGeneratorsAndUpdateCompilation`.
- For happy-path cases, the test also asserts `outputCompilation.GetDiagnostics()` contains no errors — proving the emitted `AddGeneratedServices()` extension method is not just string-matching output, but valid C# that actually compiles against a real `Microsoft.Extensions.DependencyInjection` reference.

Metadata references for the synthetic compilation are gathered at test-run time from `AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")` (every assembly the test host already loaded, including `Microsoft.Extensions.DependencyInjection`/`.Abstractions` pulled in via a package reference). This avoids adding a reference-assembly package (e.g. `Basic.Reference.Assemblies`) or a generator-testing framework (e.g. `Microsoft.CodeAnalysis.Testing`) — the only new package is `Microsoft.CodeAnalysis.CSharp`, which the generator project itself already depends on.

Internal extraction/rendering classes are deliberately left un-tested in isolation and without `InternalsVisibleTo`: black-box testing through the generator's public API validates the behavior actually shipped (attribute recognition, diagnostic wiring, and source rendering together), and avoids maintaining a second, internals-only testing seam that could pass while the real generator pipeline is broken.

### Project structure

Both new projects follow ADR-006's `tests/{mirroring-src}/{ProjectUnderTest}.Tests` layout and are registered in `NexusAiSupportLite.slnx` under a matching solution folder:

```
tests/Services/Identity/NexusSupport.Identity.Application.Tests/
tests/Shared/NexusSupport.SourceGenerator.Tests/
```

`/tests/Shared/` is a new solution folder, introduced because `/src/Shared/` (containing `NexusSupport.SourceGenerator`) previously had no test counterpart.

## Consequences

### Positive

- Both reconsideration triggers from ADR-006 are resolved without weakening its MIT-only dependency constraint — no new non-MIT package was introduced, and the only new package (`Microsoft.CodeAnalysis.CSharp`) was already a transitive necessity for anything touching the generator.
- The generator tests assert on real compiler output (including a full round-trip compile), which is stronger evidence of correctness than asserting against `internal` extraction/rendering types directly.
- The Identity Application tests reuse the exact same Moq/xUnit idioms as the Infrastructure tests, so no new onboarding cost for contributors already familiar with ADR-006.

### Negative

- Constructing a `CSharpCompilation` and driving `CSharpGeneratorDriver` by hand is more verbose per test than a purpose-built snapshot-testing harness (e.g. `Microsoft.CodeAnalysis.Testing`'s `CSharpSourceGeneratorTest<T>`), and each test re-implements comparable setup, mitigated by a small private helper method.
- Gathering metadata references via `TRUSTED_PLATFORM_ASSEMBLIES` is a runtime-environment-dependent technique; if the test host's assembly-loading behavior changes across SDK versions, reference resolution could need adjustment.
- Each Application-layer service test class hand-rolls its own DTO/model equality assertions (no reflection-based comparer), so adding a field to a model/DTO pair requires updating the corresponding test's assertions too.

## Alternatives Considered

### `InternalsVisibleTo` + direct unit tests of `ServiceRegistrationExtractor`/`ServiceRegistrationSourceBuilder`

Rejected because it tests implementation details rather than the generator's actual contract, and still would not exercise `ServiceRegistrationGenerator.Initialize`'s pipeline wiring (post-init source registration, diagnostic reporting, source production) — the exact area most likely to regress silently.

### `Microsoft.CodeAnalysis.Testing` (`Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing`)

Considered for its purpose-built generator-testing harness (expected-source diffing, built-in reference-assembly sets). Deferred for now since the hand-rolled `CSharpGeneratorDriver` approach fully covers current needs with one fewer dependency; revisit if more generators are added and the boilerplate becomes costly to duplicate.

### `Basic.Reference.Assemblies` for compilation references

Considered as a more deterministic alternative to `TRUSTED_PLATFORM_ASSEMBLIES`. Deferred as unnecessary while a single target framework (`net10.0`) is in play; noted as a fallback if reference gathering proves flaky in CI.

## Reconsideration Triggers

Reconsider this decision if:

- A second source generator is added and the per-test compilation/driver setup becomes significant duplicated boilerplate — consider extracting a shared test-support project or adopting `Microsoft.CodeAnalysis.Testing`.
- `TRUSTED_PLATFORM_ASSEMBLIES`-based reference gathering becomes unreliable in CI or across SDK versions — switch to `Basic.Reference.Assemblies`.
- Identity Application services grow logic beyond repository delegation and DTO mapping (e.g. cross-repository orchestration, validation) such that Moq-based repository mocking is no longer sufficient.

## Validation Criteria

1. `tests/Shared/NexusSupport.SourceGenerator.Tests` and `tests/Services/Identity/NexusSupport.Identity.Application.Tests` exist and are registered in `NexusAiSupportLite.slnx` under matching solution folders.
2. Generator tests cover: the `ServiceAttribute.g.cs` post-init source always being emitted; `Scoped`/`Transient`/`Singleton` lifetime rendering; self-registration when no service type is declared; the `NSG001` diagnostic when a class does not implement its declared service type; ordinal ordering of multiple registrations; assembly-name sanitization; and that emitted registration code compiles without errors.
3. Application-layer tests mock the Domain repository interface for every one of the six Identity Application services and assert both the DTO↔model mapping and delegation for every public method.
4. `dotnet test` passes for both new projects without a live database, Docker, or other external dependency.
5. No package outside the MIT-licensed set already established by ADR-006 (plus `Microsoft.CodeAnalysis.CSharp`, already required by the generator itself) is introduced.
