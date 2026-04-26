# Getting Started

A new contributor should be able to clone, install, and run smoke tests in **under 10 minutes**.

## Prerequisites

- **.NET 8 SDK** — https://dotnet.microsoft.com/download
- **PowerShell 7+** (Windows / macOS / Linux)
- **Allure CLI** *(optional, for local report viewing)* — https://allurereport.org/docs/install/
- **Docker** *(optional, for containerised runs)*

## 1. Clone

```bash
git clone <repository-url>
cd PlaywrightWithDotnet
```

## 2. Build

```powershell
dotnet build PlaywrightWithDotnet.slnx -c Release
```

## 3. Install Playwright browsers

```powershell
pwsh ./scripts/install-browsers.ps1
```

This installs Chromium, Firefox, and WebKit binaries into Playwright's local cache.

## 4. Run smoke tests

```powershell
pwsh ./scripts/run-tests.ps1 -Suite All -Category Smoke -Browsers Chromium -Environment qa
```

The default sample target is the practise app at `https://gauravkhurana.com/practise-api/`.

## 5. View the report

```powershell
pwsh ./scripts/allure-serve.ps1
```

Or run Allure inside Docker (no local install required):

```powershell
docker compose -f docker/docker-compose.yml up -d allure
# open http://localhost:5252
```

## 6. Switch environment

```powershell
$env:TEST_ENV = "dev"
pwsh ./scripts/run-tests.ps1 -Environment dev
```

## 7. Run multiple browsers in parallel

```powershell
pwsh ./scripts/run-tests.ps1 -Browsers Chromium,Firefox,Webkit -Suite UI -Category All
```

## Next steps

- Read [architecture.md](architecture.md) to understand the module layout.
- Read [writing-ui-tests.md](writing-ui-tests.md) to add your first test.
- Read [configuration.md](configuration.md) to learn the settings system.
