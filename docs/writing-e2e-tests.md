# Writing E2E Tests

E2E fixtures inherit `UiTestBase` and resolve `ApiClientFactory` from the shared DI scope to combine UI and API in one journey.

```csharp
[AllureNUnit]
[Category("E2E")]
[TestFixtureSource(typeof(UiTestFixtureSource))]
public sealed class CreateOrderJourney : UiTestBase
{
    public CreateOrderJourney(BrowserKind kind) : base(kind) { }

    [Test]
    public async Task Order_Created_Via_Api_Appears_In_Ui()
    {
        // 1. Seed via API
        var apiFactory = Services.GetRequiredService<ApiClientFactory>();
        await using var api = await apiFactory.CreateAsync();
        var seedResp = await api.SendAsync(new ApiRequest
        {
            Verb = HttpVerb.Post,
            Path = "/orders",
            Body = new { sku = "ABC", qty = 2 },
        });
        seedResp.IsSuccess.Should().BeTrue();
        var orderId = seedResp.As<Order>()!.Id;

        // 2. Verify in UI
        await Page.GotoAsync("#/orders");
        await Page.GetByText(orderId.ToString()).WaitForAsync();
    }
}
```

Best practices:

- Prefer **API for setup/teardown**; UI for verifying user-facing behaviour.
- Always clean up created records in `OnTearDownAsync` if your environment is shared.
- Use `[Order(...)]` only when sequencing within a fixture is genuinely required.
