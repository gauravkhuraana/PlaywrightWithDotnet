# Running in CI

## GitHub Actions

| Workflow | Trigger | Purpose |
| --- | --- | --- |
| `pr.yml` | pull_request | Smoke against chromium only |
| `ci.yml` | push, dispatch | Matrix (ubuntu/windows × chromium/firefox/webkit), publishes Allure to `gh-pages` |
| `nightly.yml` | cron 02:00 UTC | Full regression × all browsers |

Manual runs accept `environment` and `category` inputs via the **Run workflow** button.

### Secrets

| Secret | Used by | Notes |
| --- | --- | --- |
| `SLACK_WEBHOOK_URL` | notifier | optional, set as `PWFX_Framework__Notifications__WebhookUrl` |
| `TEAMS_WEBHOOK_URL` | notifier | optional |
| `AZURE_*` (federated) | KeyVault config provider | use OIDC with `azure/login@v2` rather than long-lived secrets |

## Azure DevOps

Entry point: [azure-pipelines.yml](../azure-pipelines.yml). Uses the reusable template under `ci/azure-pipelines/templates/test-job.yml`.

Key variables / parameters:

- `environment` — `dev | qa | staging | prod`
- `category` — `Smoke | Regression | E2E | Quality | All`
- `browsers` — list, e.g. `[Chromium, Firefox]`

Publishes:

- VSTest TRX — appears in the Tests tab.
- Allure raw results — pipeline artefact `allure-<browser>` (use Allure-on-Azure-DevOps extension or Allure docker service to render).
- Failure artefacts — `failures-<browser>` artefact with traces, videos, screenshots, HARs, logs.

### Federated identity for Azure Key Vault

Configure a workload-identity federated credential on your service principal pointing at the pipeline. Use `AzureCLI@2` (or `azure/login@v2` in GitHub) to grant `DefaultAzureCredential` an OIDC token — no secrets in pipeline variables.
