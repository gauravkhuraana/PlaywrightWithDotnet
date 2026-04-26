# Configuration Reference

All settings live under the `Framework` section.

## Layering

In order of precedence (later overrides earlier):

1. `appsettings.json`
2. `appsettings.{TEST_ENV}.json` (e.g. `appsettings.qa.json`)
3. `appsettings.Local.json` (gitignored, developer overrides)
4. Environment variables prefixed `PWFX_` with `__` as the section separator
5. Azure Key Vault (when enabled)

### Env-var examples

```bash
PWFX_Framework__Ui__Headless=false
PWFX_Framework__Ui__BaseUrl=https://staging.example.com
PWFX_Framework__Browser__Enabled__0=Firefox
PWFX_Framework__Browser__Enabled__1=Webkit
PWFX_Framework__Notifications__Channel=Slack
PWFX_Framework__Notifications__WebhookUrl=https://hooks.slack.com/services/...
```

## Settings

### `Framework`

| Key | Type | Default | Description |
| --- | --- | --- | --- |
| `Environment` | string | `qa` | Logical environment label. Drives Key Vault selection and storage-state file naming. |
| `DefaultCategory` | string | `Smoke` | Default NUnit category when none is supplied. |

### `Framework:Ui`

| Key | Default | Description |
| --- | --- | --- |
| `BaseUrl` | — | Used as `BrowserContextOptions.BaseURL`. |
| `DefaultTimeoutMs` | `30000` | Page-level default timeout. |
| `NavigationTimeoutMs` | `60000` | Navigation timeout. |
| `Headless` | `true` | Headless browser mode. |
| `SlowMo` / `SlowMoMs` | `false` / `0` | Slow Playwright actions for debugging. |
| `Locale` | `en-US` | |
| `TimezoneId` | `UTC` | |
| `Viewport.Width/Height` | `1920` / `1080` | |

### `Framework:Api`

| Key | Default | Description |
| --- | --- | --- |
| `BaseUrl` | — | API base URL prepended to relative paths. |
| `TimeoutMs` | `30000` | Per-request timeout. |
| `DefaultHeaders` | `{}` | Sent with every request. |
| `AuthScheme` / `AuthToken` | `null` | If both set, `Authorization: <scheme> <token>` is added. |

### `Framework:Browser`

| Key | Default | Description |
| --- | --- | --- |
| `Enabled` | `["Chromium"]` | Drives `UiTestFixtureSource`. Allowed: `Chromium`, `Firefox`, `Webkit`, `Edge`, `Chrome`. |
| `RecordVideo` | `false` | Record video for every test. |
| `RecordVideoOnFailureOnly` | `true` | Only retain video on failure. |
| `RecordTrace` | `true` | Capture Playwright trace. |
| `RecordTraceOnFailureOnly` | `true` | Stop & save the trace only when the test fails. |
| `ScreenshotOnFailure` | `true` | Save full-page screenshot on failure. |
| `RecordHar` | `false` | Capture network HAR. |
| `MobileProfiles` | `[]` | Enabled mobile emulation profiles (`iphone-13`, `pixel-7`, `ipad-pro-11`). |

### `Framework:Reporting`

| Key | Default | Description |
| --- | --- | --- |
| `AllureResultsDirectory` | `allure-results` | Output dir for Allure raw results. |
| `Attach{Trace,Video,Screenshot,Har,ConsoleLog}` | `true` | Toggle individual attachment types. |

### `Framework:Retry`

| Key | Default | Description |
| --- | --- | --- |
| `MaxRetries` | `1` | NUnit `[Retry]` count for flaky tests. |
| `ApiMaxRetries` | `3` | Polly retry count on `ApiClient`. |
| `ApiBaseDelayMs` | `500` | Exponential backoff base delay. |

### `Framework:Database`

| Key | Default | Description |
| --- | --- | --- |
| `Provider` | `SqlServer` | `SqlServer` or `Postgres`. |
| `ConnectionString` | `null` | Plain or Key Vault secret reference. |

### `Framework:Notifications`

| Key | Default | Description |
| --- | --- | --- |
| `Channel` | `None` | `None`, `Slack`, or `Teams`. |
| `WebhookUrl` | `null` | Webhook target. |
| `OnlyOnFailure` | `true` | Skip notification when 0 failures. |

### `Framework:Visual`

| Key | Default | Description |
| --- | --- | --- |
| `BaselinesDirectory` | `tests/.baselines` | Per-browser baselines. |
| `ActualDirectory` / `DiffDirectory` | `tests/.visual/...` | Outputs. |
| `PixelTolerance` | `0.1` | Max diff ratio (e.g. 0.1 = 10%). |
| `UpdateBaselines` | `false` | When true, captured screenshot replaces baseline. |

### `Framework:Accessibility`

| Key | Default | Description |
| --- | --- | --- |
| `Enabled` | `true` | Run scans. |
| `MinimumImpact` | `serious` | `minor`, `moderate`, `serious`, `critical`. |
| `Tags` | `["wcag2a","wcag2aa"]` | Axe rule tags. |

### `Framework:Performance`

| Key | Default | Description |
| --- | --- | --- |
| `Enabled` | `false` | Apply Web-Vitals thresholds. |
| `MaxLcpMs` / `MaxClsScore` / `MaxTtfbMs` | `2500` / `0.1` / `800` | Hard thresholds. |

### `Framework:KeyVault`

| Key | Default | Description |
| --- | --- | --- |
| `Enabled` | `false` | Add Azure Key Vault config provider. |
| `VaultUri` | `null` | E.g. `https://my-vault.vault.azure.net/`. Auth via `DefaultAzureCredential`. |
