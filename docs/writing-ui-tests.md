# Writing UI Tests

## Anatomy

```csharp
[AllureNUnit]
[Category("Smoke")]
[TestFixtureSource(typeof(UiTestFixtureSource))]   // run once per enabled browser
[Parallelizable(ParallelScope.Fixtures)]
public sealed class MyPageTests : UiTestBase
{
    public MyPageTests(BrowserKind kind) : base(kind) { }

    [Test]
    public async Task Loads_Successfully()
    {
        var page = new MyPage(Page, Logger);
        await page.GoToAsync();
        Page.Url.Should().Contain("my-page");
    }
}
```

`UiTestBase` provides:

- `Page` / `Context` — fresh per test.
- `Services` — DI scope.
- `Logger` — class-named Serilog logger.
- `Settings` — strongly-typed `FrameworkSettings`.
- `ExecutionContext` — flowing context with artefact paths.

## Page Objects

```csharp
public sealed class MyPage : PageBase
{
    public MyPage(IPage page, ILogger logger) : base(page, logger) { }
    public override string RelativeUrl => "/my-page";

    public ILocator Heading => Page.GetByRole(AriaRole.Heading).First;
    public ILocator SubmitButton => Page.GetByRole(AriaRole.Button, new() { Name = "Submit" });

    public async Task SubmitAsync() => await SubmitButton.ClickAsync();

    protected override async Task VerifyLoadedAsync()
        => await Heading.WaitForAsync();
}
```

Reusable widgets inherit `ComponentBase` and take a root `ILocator`.

## Multi-browser

Set the enabled browsers in `appsettings.{env}.json`:

```json
"Framework": {
  "Browser": { "Enabled": ["Chromium", "Firefox", "Webkit"] }
}
```

Or via env var: `PWFX_Framework__Browser__Enabled__0=Firefox`.

## Storage state (skip login)

1. Implement a one-time login fixture that calls `StorageStateManager.PathFor("admin")` and saves state via `Context.StorageStateAsync(new() { Path = pathFromManager })`.
2. Tag fixtures or methods with `[UseStorageState("admin")]` — `UiTestBase` will load the file automatically.

## Visual regression

```csharp
var verifier = Services.GetRequiredService<VisualVerifier>();
var result = await verifier.VerifyAsync(Page, nameof(My_Test), BrowserKind.ToString());
result.Matches.Should().BeTrue(result.Message);
```

Set `Framework:Visual:UpdateBaselines=true` once to refresh.

## Accessibility

```csharp
var scanner = Services.GetRequiredService<AccessibilityScanner>();
var report = await scanner.ScanAsync(Page);
scanner.GetSignificantViolations(report).Should().BeEmpty();
```

## Performance

```csharp
var probe = Services.GetRequiredService<PerformanceProbe>();
var vitals = await probe.CaptureAsync(Page);
probe.EvaluateThresholds(vitals).Should().BeEmpty();
```
