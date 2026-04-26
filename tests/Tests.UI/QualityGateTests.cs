using Allure.NUnit;
using Allure.NUnit.Attributes;
using FluentAssertions;
using Framework.UI.Accessibility;
using Framework.UI.Browser;
using Framework.UI.Testing;
using Framework.UI.Visual;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Tests.UI.Pages;

namespace Tests.UI;

/// <summary>Showcases visual regression and accessibility scanning helpers.</summary>
[AllureNUnit]
[AllureSuite("UI")]
[AllureFeature("Quality Gates")]
[Category("Quality")]
[TestFixtureSource(typeof(UiTestFixtureSource))]
[Parallelizable(ParallelScope.Fixtures)]
public sealed class QualityGateTests : UiTestBase
{
    public QualityGateTests(BrowserKind kind) : base(kind) { }

    [Test]
    [AllureStory("Visual baseline check")]
    public async Task Visual_Baseline_For_Scenarios_Page()
    {
        var page = new ScenariosPage(Page, Logger);
        await page.GoToScenariosAsync();

        var verifier = Services.GetRequiredService<VisualVerifier>();
        var result = await verifier.VerifyAsync(Page, nameof(Visual_Baseline_For_Scenarios_Page), BrowserKind.ToString());

        // First-run creates baseline (Matches=true). Subsequent runs do real comparison.
        result.Matches.Should().BeTrue(result.Message);
    }

    [Test]
    [AllureStory("Accessibility scan")]
    public async Task Accessibility_Scan_Reports_No_Critical_Violations()
    {
        var page = new ScenariosPage(Page, Logger);
        await page.GoToScenariosAsync();

        var scanner = Services.GetRequiredService<AccessibilityScanner>();
        var result = await scanner.ScanAsync(Page);
        var significant = scanner.GetSignificantViolations(result);

        significant.Should().BeEmpty(
            "no accessibility violations at or above the configured impact threshold are allowed; " +
            "found: " + string.Join(", ", significant.Select(v => v.Id)));
    }
}
