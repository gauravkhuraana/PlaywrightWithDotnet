# Architecture

## Module diagram

```
┌────────────────────────────────────────────────────────────┐
│                       Test Projects                        │
│  Tests.UI   Tests.Api   Tests.E2E   Tests.Bdd  (Tests.Common) │
└──────────────┬───────────────────────┬─────────────────────┘
               │                       │
       ┌───────┴───────┐       ┌───────┴───────┐
       │  Framework.UI │       │ Framework.Api │
       │ Pages, Browser│       │ ApiClient,    │
       │ Visual, A11y  │       │ Schema, Polly │
       └───────┬───────┘       └───────┬───────┘
               │                       │
       ┌───────┴───────────────────────┴───────┐
       │            Framework.Core             │
       │  DI, Hooks, TestBase, Playwright SP   │
       │           (logging via Serilog)       │
       └───────┬─────────────────┬─────────────┘
               │                 │
   ┌───────────┴───┐   ┌─────────┴───────┐   ┌──────────────┐
   │ Framework.    │   │ Framework.      │   │ Framework.   │
   │ Configuration │   │ Reporting       │   │ Data         │
   │ (KeyVault)    │   │ (Allure,Notify) │   │ (Bogus,Db)   │
   └───────────────┘   └─────────────────┘   └──────────────┘
                       Framework.Utilities (shared)
```

## Dependency Injection

`FrameworkServiceProvider.Root` builds a singleton `IServiceProvider` once per process. Each test creates a **child scope** in `TestBase.SetUp` and disposes it in `TearDown`. Feature modules (`FrameworkUiModule`, `FrameworkApiModule`, `FrameworkReportingModule`) register their services via `FrameworkServiceRegistration.AddRegistration(...)`.

## Hook pipeline

The `HookPipeline` class implements a **chain of responsibility** over `ITestHook` instances. `Before` runs in ascending `Order`, `After` in descending. Built-in hooks include:

| Hook | Order | Purpose |
| --- | ---: | --- |
| `AllureAttachmentsHook` | 100 | Attach screenshot/trace/video/HAR on failure |

Add your own by implementing `ITestHook` and registering it in a feature module.

## Design patterns

| Pattern | Where |
| --- | --- |
| **Page Object + Component Object** | `Framework.UI.Pages.PageBase` / `ComponentBase` |
| **Factory** | `BrowserFactory`, `BrowserContextFactory`, `ApiClientFactory` |
| **Singleton (scoped)** | `PlaywrightFactory`, `IConfiguration`, `FrameworkSettings` |
| **Strategy** | `INotifier` (Slack, Teams, NoOp) |
| **Builder** | `TestDataBuilder<TBuilder, TResult>`, fluent `ApiRequest` |
| **Adapter** | `AllureAttachmentsHook` (framework events → Allure attachments) |
| **Chain of Responsibility** | `HookPipeline` |
| **Decorator** | DI logging wrapper around `ApiClient` (extension point) |

## Parallelism

- Each test assembly enables `[assembly: Parallelizable(ParallelScope.Fixtures)]` and `[assembly: LevelOfParallelism(4)]`.
- UI fixtures parameterised over enabled browsers via `[TestFixtureSource(typeof(UiTestFixtureSource))]` — each `(BrowserKind, fixture)` pair runs concurrently.
- API tests use `ParallelScope.All` to run individual tests in parallel.
- Per-test browser **context** (not browser instance) ensures isolation without paying full launch cost.

## Configuration flow

1. `ConfigurationLoader.Build()` loads JSON layers + env vars.
2. If `Framework:KeyVault:Enabled`, an Azure Key Vault provider is added with `DefaultAzureCredential`.
3. `FrameworkSettings` is bound and registered as a singleton + via `IOptions<T>`.
4. Modules read `FrameworkSettings` at registration time to decide which strategies to wire (e.g. Slack vs Teams notifier).

## Failure artefact pipeline

```
Test fails  →  UiTestBase.OnTearDownAsync
            →  capture trace.zip, failure.png, network.har, video/*.webm
            →  AllureAttachmentsHook.AfterAsync
            →  AllureApi.AddAttachment(...) per artefact
```

## Extending

- New module → new project under `src/`, expose a `Register()` static.
- New hook → implement `ITestHook`, register in a module, set `Order` carefully.
- New reporter → implement `INotifier`, switch on configuration value in module registration.
