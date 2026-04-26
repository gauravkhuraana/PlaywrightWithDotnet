[CmdletBinding()]
param(
    [string]$ResultsDirectory = "allure-results"
)
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try
{
    if (-not (Get-Command allure -ErrorAction SilentlyContinue))
    {
        throw "Allure CLI is not installed. See https://allurereport.org/docs/install/"
    }
    allure serve $ResultsDirectory
}
finally
{
    Pop-Location
}
