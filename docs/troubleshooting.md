# Troubleshooting

## "Executable doesn't exist" on first run

Run `pwsh ./scripts/install-browsers.ps1`. The script invokes `playwright.ps1 install --with-deps` from the Tests.UI build output.

## Tests hang on `WaitForLoadStateAsync`

Single-page apps may never reach `NetworkIdle`. Use `LoadState.DOMContentLoaded` plus an explicit locator wait instead.

## `BrowserType` ambiguous reference

Use `BrowserKind` — the framework's enum. `Microsoft.Playwright.BrowserType` is the SDK class for the browser **launcher**.

## Tests fail with `Target page, context or browser has been closed`

The fixture-level browser was disposed mid-test. Make sure your test isn't capturing `Page` from another fixture or running over the configured timeout.

## Trace viewer shows nothing

`Framework:Browser:RecordTrace` must be `true`. With `RecordTraceOnFailureOnly=true`, traces are only saved for failing tests.

## Visual diff always fails by 1 pixel

Increase `Framework:Visual:PixelTolerance`, or commit a fresh baseline by running once with `Framework:Visual:UpdateBaselines=true`.

## Allure report is empty

Confirm `allureConfig.json` is copied to the test output and that `allure-results` actually contains JSON files. Check that `[AllureNUnit]` is on each fixture.

## Key Vault auth fails locally

Run `az login` first. `DefaultAzureCredential` falls back to Azure CLI credentials when no managed identity is present.

## Reqnroll bindings not discovered

Confirm the project references `Reqnroll.NUnit` *and* `Reqnroll.Tools.MsBuild.Generation`. Re-build to regenerate `.feature.cs` files.

## Running on Apple Silicon / Linux ARM

Playwright supports both. Use the `mcr.microsoft.com/playwright/dotnet:v1.49.0-jammy` image (multi-arch) and avoid `WebKit` on ARM Linux containers if you hit headless issues.
