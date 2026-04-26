using Framework.UI.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Tests.UI.Pages;

/// <summary>The Scenarios page (the "#/scenarios" route of the practise UI).</summary>
public sealed class ScenariosPage : PageBase
{
    public ScenariosPage(IPage page, ILogger logger) : base(page, logger) { }

    public override string RelativeUrl => "#/scenarios";

    /// <summary>Top-level title heading.</summary>
    public ILocator Heading => Page.GetByRole(AriaRole.Heading).First;

    /// <summary>Generic content container.</summary>
    public ILocator Content => Page.Locator("body");

    public Task GoToScenariosAsync() => GoToAsync();

    protected override async Task VerifyLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
    }
}
