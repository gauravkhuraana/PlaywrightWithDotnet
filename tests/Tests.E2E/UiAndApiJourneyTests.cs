using Allure.NUnit;
using Allure.NUnit.Attributes;
using FluentAssertions;
using Framework.Api.Http;
using Framework.Core.Testing;
using Framework.UI.Browser;
using Framework.UI.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Tests.E2E;

/// <summary>
/// End-to-end journey combining the practise UI and API:
/// 1. UI loads the scenarios page.
/// 2. API health is verified at the same base host.
/// Together this proves both stacks share a consistent execution context.
/// </summary>
[AllureNUnit]
[AllureSuite("E2E")]
[AllureFeature("Practise UI + API")]
[Category("E2E")]
[TestFixtureSource(typeof(UiTestFixtureSource))]
[Parallelizable(ParallelScope.Fixtures)]
public sealed class UiAndApiJourneyTests : UiTestBase
{
    public UiAndApiJourneyTests(BrowserKind kind) : base(kind) { }

    [Test]
    [AllureStory("UI loads and API responds")]
    public async Task Ui_And_Api_Are_Both_Available()
    {
        // UI step
        await Page.GotoAsync("#/scenarios", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        Page.Url.Should().Contain("scenarios");

        // API step (using the same DI scope as the UI fixture)
        var apiFactory = Services.GetRequiredService<Framework.Api.Http.ApiClientFactory>();
        await using var api = await apiFactory.CreateAsync();
        var response = await api.SendAsync(new ApiRequest { Verb = HttpVerb.Get, Path = "/" });
        response.StatusCode.Should().BeLessThan(500);
    }
}
