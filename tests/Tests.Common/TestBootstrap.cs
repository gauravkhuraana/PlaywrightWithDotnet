using Framework.Api;
using Framework.Reporting;
using Framework.UI;

namespace Tests.Common;

/// <summary>
/// Process-wide test bootstrap — registers all framework modules before any test runs.
/// </summary>
public static class TestBootstrap
{
    private static int _initialized;

    /// <summary>Idempotent. Call once from a fixture's <c>OneTimeSetUp</c> or via the [SetUpFixture].</summary>
    public static void Initialize()
    {
        if (Interlocked.Exchange(ref _initialized, 1) == 1)
        {
            return;
        }
        FrameworkUiModule.Register();
        FrameworkApiModule.Register();
        FrameworkReportingModule.Register();
    }
}

[NUnit.Framework.SetUpFixture]
public sealed class GlobalTestBootstrap
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup() => TestBootstrap.Initialize();
}
