using Framework.Configuration;
using Framework.Configuration.Models;
using Framework.UI.Browser;

namespace Framework.UI.Testing;

/// <summary>
/// NUnit <c>TestFixtureSource</c> provider that yields the configured set of browsers,
/// enabling <c>[TestFixtureSource(typeof(UiTestFixtureSource))]</c> on UI fixtures
/// to run them across every enabled browser in parallel.
/// </summary>
public sealed class UiTestFixtureSource : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var config = ConfigurationLoader.Build();
        var settings = ConfigurationLoader.BindSettings(config);
        foreach (var name in settings.Browser.Enabled)
        {
            yield return new object[] { BrowserKindParser.Parse(name) };
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
