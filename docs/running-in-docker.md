# Running in Docker

The image extends Microsoft's official Playwright .NET base, so all browsers are pre-installed.

## Build

```powershell
docker build -t playwright-fx -f docker/Dockerfile .
```

## Run

```powershell
docker run --rm `
  -e TEST_ENV=qa `
  -e TEST_CATEGORY=Smoke `
  -e BROWSERS=Chromium,Firefox `
  -v ${PWD}/TestResults:/app/TestResults `
  -v ${PWD}/allure-results:/app/allure-results `
  playwright-fx
```

Environment variables consumed by `docker/entrypoint.sh`:

| Var | Default | Notes |
| --- | --- | --- |
| `TEST_ENV` | `qa` | Selects `appsettings.{env}.json`. |
| `TEST_CATEGORY` | `Smoke` | Maps to `--filter "Category=..."`. |
| `TEST_SUITE` | `All` | `UI`, `Api`, `E2E`, `Bdd`, or `All`. |
| `BROWSERS` | `Chromium` | Comma-separated; populates `PWFX_Framework__Browser__Enabled__N`. |

## Compose (tests + Allure UI)

```powershell
docker compose -f docker/docker-compose.yml up tests
docker compose -f docker/docker-compose.yml up -d allure
# open http://localhost:5252
```

Mount `./allure-results` into the `allure` service to make new results appear automatically.
