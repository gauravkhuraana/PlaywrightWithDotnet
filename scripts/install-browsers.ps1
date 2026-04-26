[CmdletBinding()]
param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build (Join-Path $root "PlaywrightWithDotnet.slnx") -c $Configuration | Out-Host

$uiAsm = Join-Path $root "tests/Tests.UI/bin/$Configuration/net8.0/Tests.UI.dll"
if (-not (Test-Path $uiAsm))
{
    throw "Tests.UI assembly not found at $uiAsm"
}

Write-Host "Installing Playwright browsers..." -ForegroundColor Cyan
$ps1 = Join-Path $root "tests/Tests.UI/bin/$Configuration/net8.0/playwright.ps1"
if (-not (Test-Path $ps1))
{
    throw "playwright.ps1 not found. Did the build succeed?"
}

pwsh -File $ps1 install --with-deps
