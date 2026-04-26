using Framework.Configuration.Models;
using Microsoft.Playwright;

namespace Framework.UI.Browser;

/// <summary>Options influencing per-test browser context creation.</summary>
public sealed class BrowserContextOptions
{
    public string? StorageStatePath { get; init; }
    public string? MobileProfile { get; init; }
    public bool ForceRecordTrace { get; init; }
    public bool ForceRecordVideo { get; init; }
    public bool ForceRecordHar { get; init; }
    public string? VideoDirectory { get; init; }
    public string? HarPath { get; init; }
}

/// <summary>Creates configured <see cref="IBrowserContext"/> instances per test.</summary>
public sealed class BrowserContextFactory
{
    private readonly FrameworkSettings _settings;

    public BrowserContextFactory(FrameworkSettings settings)
    {
        _settings = settings;
    }

    public async Task<IBrowserContext> CreateAsync(IBrowser browser, BrowserContextOptions? opts = null)
    {
        opts ??= new BrowserContextOptions();

        var contextOptions = new Microsoft.Playwright.BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = _settings.Ui.Viewport.Width,
                Height = _settings.Ui.Viewport.Height,
            },
            Locale = _settings.Ui.Locale,
            TimezoneId = _settings.Ui.TimezoneId,
            BaseURL = _settings.Ui.BaseUrl,
            IgnoreHTTPSErrors = true,
        };

        if (!string.IsNullOrWhiteSpace(opts.StorageStatePath) && File.Exists(opts.StorageStatePath))
        {
            contextOptions.StorageStatePath = opts.StorageStatePath;
        }

        ApplyMobileProfile(contextOptions, opts.MobileProfile);

        if ((_settings.Browser.RecordVideo && !_settings.Browser.RecordVideoOnFailureOnly) || opts.ForceRecordVideo)
        {
            contextOptions.RecordVideoDir = opts.VideoDirectory ?? Path.Combine("TestResults", "videos");
            contextOptions.RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 };
        }

        if ((_settings.Browser.RecordHar && !_settings.Browser.RecordTraceOnFailureOnly) || opts.ForceRecordHar)
        {
            contextOptions.RecordHarPath = opts.HarPath ?? Path.Combine("TestResults", "har", $"{Guid.NewGuid():N}.har");
            contextOptions.RecordHarMode = HarMode.Full;
        }

        var ctx = await browser.NewContextAsync(contextOptions).ConfigureAwait(false);
        ctx.SetDefaultTimeout(_settings.Ui.DefaultTimeoutMs);
        ctx.SetDefaultNavigationTimeout(_settings.Ui.NavigationTimeoutMs);

        if ((_settings.Browser.RecordTrace && !_settings.Browser.RecordTraceOnFailureOnly) || opts.ForceRecordTrace)
        {
            await ctx.Tracing.StartAsync(new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true,
            }).ConfigureAwait(false);
        }

        return ctx;
    }

    private static void ApplyMobileProfile(Microsoft.Playwright.BrowserNewContextOptions options, string? profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName))
        {
            return;
        }

        // Minimal built-in mobile profiles. Extendable via configuration.
        switch (profileName.Trim().ToLowerInvariant())
        {
            case "iphone-13":
                options.ViewportSize = new ViewportSize { Width = 390, Height = 844 };
                options.DeviceScaleFactor = 3;
                options.IsMobile = true;
                options.HasTouch = true;
                options.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.0 Mobile/15E148 Safari/604.1";
                break;

            case "pixel-7":
                options.ViewportSize = new ViewportSize { Width = 412, Height = 915 };
                options.DeviceScaleFactor = 2.625f;
                options.IsMobile = true;
                options.HasTouch = true;
                options.UserAgent = "Mozilla/5.0 (Linux; Android 13; Pixel 7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36";
                break;

            case "ipad-pro-11":
                options.ViewportSize = new ViewportSize { Width = 834, Height = 1194 };
                options.DeviceScaleFactor = 2;
                options.IsMobile = true;
                options.HasTouch = true;
                options.UserAgent = "Mozilla/5.0 (iPad; CPU OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.0 Mobile/15E148 Safari/604.1";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(profileName), profileName, "Unknown mobile profile.");
        }
    }
}
