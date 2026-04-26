# ADR 0004 — DI and Module pattern

## Status
Accepted

## Context
Test projects must compose framework features (UI, API, Reporting, Data) without knowing their internals. We also need per-test isolation without rebuilding the world for every test.

## Decision
Use **Microsoft.Extensions.DependencyInjection** with:

- A process-wide `FrameworkServiceProvider.Root` built once.
- Feature **modules** (`FrameworkUiModule`, `FrameworkApiModule`, `FrameworkReportingModule`) that register themselves via a static `AddRegistration(...)` extension hook so `Framework.Core` does not depend on them.
- Per-test `IServiceScope` created in `TestBase.SetUp` and disposed in `TearDown`.

## Consequences
- Modules are independently testable and replaceable.
- Each test gets fresh scoped services (e.g. its own `Page`, `IAPIRequestContext`).
- Adding a new feature = new project + new module + reference from `Tests.Common.TestBootstrap`.
