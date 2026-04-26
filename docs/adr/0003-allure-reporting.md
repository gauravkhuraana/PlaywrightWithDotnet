# ADR 0003 — Allure Reporting

## Status
Accepted

## Context
We needed a single report format that:

- Renders well in CI artefacts and on a public site (gh-pages).
- Supports rich attachments (screenshots, traces, videos, HARs, logs).
- Tracks history and trends across runs.

## Decision
Use **Allure 2** (`Allure.NUnit` + `Allure.Reqnroll`). Publish raw results from CI; render via Allure CLI locally and Allure docker service in shared environments. Push generated HTML to `gh-pages` from `main`.

## Consequences
- Free, open-source, browser-rendered.
- Failure attachments are wired through the framework's `AllureAttachmentsHook` so all test types contribute uniformly.
- TRX is still emitted for native CI Test tabs (Azure DevOps, GitHub Actions checks).
