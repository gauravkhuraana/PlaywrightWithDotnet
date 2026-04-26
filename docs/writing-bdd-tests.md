# Writing BDD Tests

Reqnroll generates NUnit fixtures from `.feature` files at build time.

## Feature file

```gherkin
# tests/Tests.Bdd/Features/Login.feature
Feature: User can log in

  @smoke @ui
  Scenario: Successful login
    Given a fresh browser session is launched
    When I navigate to the login page
    And I sign in as "admin"
    Then I should see the dashboard
```

## Step definitions

```csharp
[Binding]
public sealed class LoginSteps
{
    private readonly ScenarioContext _scenario;
    public LoginSteps(ScenarioContext scenario) => _scenario = scenario;

    [When("I sign in as {string}")]
    public async Task WhenISignIn(string user) { /* ... */ }
}
```

Steps resolve framework services from `FrameworkServiceProvider.Root` and dispose per-scenario state in `[AfterScenario]` hooks.

## Categories

Tag scenarios with Reqnroll tags (`@smoke`, `@regression`, ...). At runtime, NUnit categories are auto-mapped — filter with `--filter "Category=smoke"`.

## Allure integration

Reqnroll plus `Allure.Reqnroll` reports each scenario with full step-level detail. Failure attachments from the framework hooks still apply.
