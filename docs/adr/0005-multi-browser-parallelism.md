# ADR 0005 — Multi-browser parallelism strategy

## Status
Accepted

## Context
We need to run the same suite across Chromium, Firefox, and WebKit, in parallel, with deterministic isolation and acceptable resource usage on developer laptops and in CI.

## Decision
- One **`BrowserKind` enum** identifies which engine to launch.
- `UiTestFixtureSource` yields one fixture instance **per enabled browser** (`[TestFixtureSource(typeof(UiTestFixtureSource))]`).
- Per-test isolation via fresh **`IBrowserContext`** (cheap), not fresh browser launches.
- `[assembly: Parallelizable(ParallelScope.Fixtures)]` + `[assembly: LevelOfParallelism(4)]` per test project.
- CI uses **matrix** strategies (one job per browser) — keeps logs and artefacts per browser and uses idle agents.

## Consequences
- Linear scaling up to ~CPU-count fixtures with low memory overhead.
- Easy to add/remove browsers via `Framework:Browser:Enabled` — no code changes.
- Failure attachments (trace, video, HAR) and Allure suites segregate cleanly per browser.
