[CmdletBinding()]
param(
    [ValidateSet("Smoke", "Regression", "E2E", "Quality", "All")]
    [string]$Category = "Smoke",

    [ValidateSet("dev", "qa", "staging", "prod")]
    [string]$Environment = "qa",

    [ValidateSet("Chromium", "Firefox", "Webkit", "Edge", "Chrome")]
    [string[]]$Browsers = @("Chromium"),

    [ValidateSet("UI", "Api", "E2E", "Bdd", "All")]
    [string]$Suite = "All",

    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$env:TEST_ENV = $Environment
$env:PWFX_Framework__Browser__Enabled__0 = $Browsers -join ","

# Map enabled browsers via environment variable overrides (one entry per index).
for ($i = 0; $i -lt $Browsers.Length; $i++)
{
    Set-Item -Path "Env:PWFX_Framework__Browser__Enabled__$i" -Value $Browsers[$i]
}

$projects = switch ($Suite)
{
    "UI"  { @("tests/Tests.UI/Tests.UI.csproj") }
    "Api" { @("tests/Tests.Api/Tests.Api.csproj") }
    "E2E" { @("tests/Tests.E2E/Tests.E2E.csproj") }
    "Bdd" { @("tests/Tests.Bdd/Tests.Bdd.csproj") }
    "All" {
        @(
            "tests/Tests.UI/Tests.UI.csproj",
            "tests/Tests.Api/Tests.Api.csproj",
            "tests/Tests.E2E/Tests.E2E.csproj",
            "tests/Tests.Bdd/Tests.Bdd.csproj"
        )
    }
}

$filter = if ($Category -ne "All") { "Category=$Category" } else { $null }

foreach ($p in $projects)
{
    Write-Host "Running $p (env=$Environment, browsers=$($Browsers -join ','), filter=$filter)" -ForegroundColor Cyan
    $args = @(
        "test", (Join-Path $root $p),
        "-c", $Configuration,
        "--no-build",
        "--logger", "trx",
        "--logger", "console;verbosity=normal",
        "--results-directory", (Join-Path $root "TestResults"),
        "--settings", (Join-Path $root ".runsettings")
    )
    if ($filter) { $args += @("--filter", $filter) }
    & dotnet @args
}
