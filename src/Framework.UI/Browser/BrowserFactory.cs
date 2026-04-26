using Framework.Configuration.Models;
using Framework.Core.Playwright;
using Microsoft.Playwright;

namespace Framework.UI.Browser;

/// <summary>Creates <see cref="IBrowser"/> instances per browser type with framework-driven options.</summary>
public sealed class BrowserFactory
{
    private readonly PlaywrightFactory _playwright;
    private readonly FrameworkSettings _settings;

    public BrowserFactory(PlaywrightFactory playwright, FrameworkSettings settings)
    {
        _playwright = playwright;
        _settings = settings;
    }

    public async Task<IBrowser> LaunchAsync(BrowserKind kind)
    {
        var pw = await _playwright.GetAsync().ConfigureAwait(false);
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _settings.Ui.Headless,
            SlowMo = _settings.Ui.SlowMo ? _settings.Ui.SlowMoMs : null,
        };

        return kind switch
        {
            BrowserKind.Chromium => await pw.Chromium.LaunchAsync(launchOptions).ConfigureAwait(false),
            BrowserKind.Firefox => await pw.Firefox.LaunchAsync(launchOptions).ConfigureAwait(false),
            BrowserKind.Webkit => await pw.Webkit.LaunchAsync(launchOptions).ConfigureAwait(false),
            BrowserKind.Edge => await pw.Chromium.LaunchAsync(WithChannel(launchOptions, "msedge")).ConfigureAwait(false),
            BrowserKind.Chrome => await pw.Chromium.LaunchAsync(WithChannel(launchOptions, "chrome")).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
    }

    private static BrowserTypeLaunchOptions WithChannel(BrowserTypeLaunchOptions src, string channel)
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = src.Headless,
            SlowMo = src.SlowMo,
            Channel = channel,
        };
    }
}
