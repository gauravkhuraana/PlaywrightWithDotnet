# Contributing

Thanks for your interest in improving the framework. Please follow the conventions below so changes stay consistent.

## Branching

- `main` — protected, releasable.
- `develop` — integration.
- Feature branches: `feature/<short-topic>`; bugfixes: `fix/<short-topic>`.

## Local checks before opening a PR

```powershell
dotnet build PlaywrightWithDotnet.slnx -c Release
dotnet format PlaywrightWithDotnet.slnx --verify-no-changes
pwsh ./scripts/run-tests.ps1 -Category Smoke -Browsers Chromium
```

## Coding standards

- C# 12 / .NET 8. `Nullable` and `ImplicitUsings` are enabled solution-wide.
- Follow the `.editorconfig`. Private fields are `_camelCase`.
- Public APIs require XML doc comments.
- Prefer **fluent + immutable** APIs (records, init-only setters).
- Tests: one fixture = one feature/page/component. Each test is independent and idempotent.

## Adding a new test type

| Type | Location | Base class |
| --- | --- | --- |
| UI | `tests/Tests.UI` | `UiTestBase` (with `[TestFixtureSource(typeof(UiTestFixtureSource))]`) |
| API | `tests/Tests.Api` | `ApiTestBase` |
| E2E | `tests/Tests.E2E` | `UiTestBase` (then resolve `ApiClientFactory` from `Services`) |
| BDD | `tests/Tests.Bdd` | `[Binding]` step classes; resolve services from `FrameworkServiceProvider.Root` |

## Page Object guidelines

- One Page Object per route. Inherits `PageBase`. Override `RelativeUrl` and `VerifyLoadedAsync`.
- Reusable widgets inherit `ComponentBase` and take a root `ILocator`.
- No assertions inside page objects — they expose state; tests assert.

## Adding a configuration setting

1. Add a property to `FrameworkSettings` (or a nested type).
2. Add a default in every `appsettings.json` under `tests/`.
3. Document it in [docs/configuration.md](docs/configuration.md).

## Reporting & logs

- Use the injected `ILogger`; do not write to `Console` directly.
- Attach extra Allure attachments via `Allure.Net.Commons.AllureApi.AddAttachment(...)` from inside a hook or test.
