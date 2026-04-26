using Allure.NUnit;
using Allure.NUnit.Attributes;
using FluentAssertions;
using Framework.UI.Browser;
using Framework.UI.Testing;
using NUnit.Framework;
using Tests.UI.Pages;

namespace Tests.UI;

/// <summary>
/// Demonstrates the multi-browser fixture-source pattern: the same fixture is run once per
/// configured browser. Configure <c>Framework.Browser.Enabled</c> in appsettings to expand.
/// </summary>
[AllureNUnit]
[AllureSuite("UI")]
[AllureFeature("Practise Scenarios")]
[Category("Smoke")]
[TestFixtureSource(typeof(UiTestFixtureSource))]
[Parallelizable(ParallelScope.Fixtures)]
public sealed class ScenariosPageTests : UiTestBase
{
    public ScenariosPageTests(BrowserKind kind) : base(kind) { }

    [Test]
    [AllureStory("Scenarios page loads")]
    public async Task Scenarios_Page_Loads_Successfully()
    {
        var page = new ScenariosPage(Page, Logger);
        await page.GoToScenariosAsync();

        Page.Url.Should().Contain("scenarios");
        var title = await Page.TitleAsync();
        title.Should().NotBeNullOrEmpty();
    }

    [Test]
    [AllureStory("Page screenshot can be captured")]
    public async Task Can_Capture_Screenshot_Of_Scenarios_Page()
    {
        var page = new ScenariosPage(Page, Logger);
        await page.GoToScenariosAsync();

        var bytes = await Page.ScreenshotAsync();
        bytes.Should().NotBeEmpty();
    }
}
