# Reports

## Allure

Each test project writes raw results to its `bin/.../allure-results/` folder (configured via `allureConfig.json`). After a run:

```powershell
pwsh ./scripts/allure-serve.ps1
```

Or aggregate from CI: download every `allure-results-*` artefact, merge into one folder, then `allure generate` / `allure serve`.

### Attachments on failure

| Type | Source | Configured by |
| --- | --- | --- |
| Screenshot | `failure.png` | `Framework:Browser:ScreenshotOnFailure` |
| Trace (zip, opens in https://trace.playwright.dev) | `trace.zip` | `Framework:Browser:RecordTrace` |
| Video | `video/*.webm` | `Framework:Browser:RecordVideo` |
| HAR | `network.har` | `Framework:Browser:RecordHar` |

Toggle individual attachments under `Framework:Reporting:Attach*`.

## Logs

Serilog writes:

- Console (test output).
- Rolling file under `TestResults/logs/test-YYYYMMDD.log` (7-day retention by default).

Configure additional sinks (Seq, Application Insights) under the `Serilog` section in `appsettings.json`.

## Notifications

Configure `Framework:Notifications:Channel` (`Slack`, `Teams`, or `None`) and `WebhookUrl`. The notifier is invoked at the end of the run with a summary (total/passed/failed/skipped/duration).
