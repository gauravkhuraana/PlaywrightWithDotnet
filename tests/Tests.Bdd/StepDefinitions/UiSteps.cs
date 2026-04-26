using FluentAssertions;
using Framework.Configuration.Models;
using Framework.Core.DependencyInjection;
using Framework.Core.Playwright;
using Framework.UI.Browser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Reqnroll;

namespace Tests.Bdd.StepDefinitions;

[Binding]
public sealed class UiSteps
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    [Given("a fresh browser session is launched")]
    public async Task GivenFreshBrowser()
    {
        var sp = FrameworkServiceProvider.Root;
        var browserFactory = sp.GetRequiredService<BrowserFactory>();
        var contextFactory = sp.GetRequiredService<BrowserContextFactory>();
        _browser = await browserFactory.LaunchAsync(BrowserKind.Chromium);
        _context = await contextFactory.CreateAsync(_browser, new BrowserContextOptions());
        _page = await _context.NewPageAsync();
    }

    [When("I navigate to the scenarios page")]
    public async Task WhenINavigate()
    {
        if (_page is null)
        {
            throw new InvalidOperationException("Browser not started.");
        }
        await _page.GotoAsync("#/scenarios", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    [Then("the page URL should contain {string}")]
    public void ThenUrlContains(string fragment)
    {
        _page!.Url.Should().Contain(fragment);
    }

    [AfterScenario]
    public async Task After()
    {
        if (_context is not null)
        {
            await _context.CloseAsync();
        }
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }
    }
}
