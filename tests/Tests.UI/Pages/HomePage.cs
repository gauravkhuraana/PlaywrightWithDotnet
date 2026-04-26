using Framework.UI.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Tests.UI.Pages;

/// <summary>Landing page of the practise UI.</summary>
public sealed class HomePage : PageBase
{
    public HomePage(IPage page, ILogger logger) : base(page, logger) { }

    public override string RelativeUrl => string.Empty;

    public ILocator AppRoot => Page.Locator("body");
}
