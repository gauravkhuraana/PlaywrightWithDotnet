# Changelog

All notable changes are documented here. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/).

## [1.0.0] — Initial release

### Added
- Solution scaffold with 7 framework projects and 5 test projects under `PlaywrightWithDotnet.slnx`.
- `Framework.Configuration` — layered JSON + env-var + Azure Key Vault provider, strongly-typed `FrameworkSettings`.
- `Framework.Core` — DI host, hook pipeline, scoped Playwright lifetime, `TestBase`.
- `Framework.UI` — Playwright browser/context factories, Page/Component objects, storage-state reuse, visual regression (ImageSharp), Axe accessibility, Web-Vitals performance probe.
- `Framework.Api` — fluent `ApiClient` with Polly retries, JSON-Schema validation, correlation IDs.
- `Framework.Data` — Bogus fakers, generic test-data builder, `IDbVerifier` (SQL Server + Postgres via Dapper).
- `Framework.Reporting` — Allure attachments hook (screenshot/trace/video/HAR), Slack and Teams notifiers.
- `Framework.Utilities` — JSON defaults, retry policies, path helpers.
- Sample test projects covering API, UI (multi-browser fixture), E2E, BDD (Reqnroll).
- Docker image (Playwright .NET base + Allure CLI), `entrypoint.sh`, and Compose stack for tests + Allure UI.
- GitHub Actions workflows: `pr.yml`, `ci.yml` (matrix + gh-pages), `nightly.yml`.
- Azure DevOps pipeline with reusable `test-job.yml` template.
- PowerShell scripts: `install-browsers.ps1`, `run-tests.ps1`, `allure-serve.ps1`.
- Documentation site under `docs/` with Getting Started, Architecture, Configuration Reference, test-authoring guides, Reports, CI, Docker, Troubleshooting, and ADRs.
