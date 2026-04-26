# Writing API Tests

## Anatomy

```csharp
[AllureNUnit]
[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
public sealed class UsersApiTests : ApiTestBase
{
    [Test]
    public async Task Get_User_Returns_200()
    {
        var response = await Api.SendAsync(new ApiRequest
        {
            Verb = HttpVerb.Get,
            Path = "/users/1",
        });

        response.IsSuccess.Should().BeTrue();
        response.As<User>()!.Id.Should().Be(1);
    }
}
```

`ApiTestBase` exposes a per-test `ApiClient` (`Api`).

## Request features

```csharp
new ApiRequest
{
    Verb = HttpVerb.Post,
    Path = "/users",
    Headers = new() { ["X-Tenant"] = "acme" },
    QueryParams = new() { ["draft"] = "true" },
    Body = new { name = "Gaurav", role = "automation" },
    CorrelationId = "explicit-id-if-needed",
    Timeout = TimeSpan.FromSeconds(10),
}
```

A `X-Correlation-Id` header is added automatically when not set.

## Authentication

Set `Framework:Api:AuthScheme` and `Framework:Api:AuthToken`. The client adds `Authorization: <scheme> <token>` to every request. Pull the token from Azure Key Vault by enabling the Key Vault provider.

## Schema validation

```csharp
var errors = JsonSchemaValidator.Validate(response.Body, "schemas/user.json");
errors.Should().BeEmpty();
```

## Retries

`ApiClient` runs every request through a Polly exponential-backoff pipeline (count + base delay configured under `Framework:Retry`).

## Database verification

```csharp
var db = Services.GetRequiredService<IDbVerifier>();
var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users WHERE Id = @id", new { id = 1 });
count.Should().Be(1);
```
