using Framework.Configuration.Models;
using Framework.Core.Hooks;
using Framework.Core.Testing;
using Framework.UI.Attributes;
using Framework.UI.Browser;
using Framework.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Framework.UI.Testing;

/// <summary>
/// Base fixture for UI tests. Each test gets its own <see cref="IBrowserContext"/> + <see cref="IPage"/>.
/// Fixtures are parameterised by browser via <see cref="UiTestFixtureSource"/>.
/// </summary>
public abstract class UiTestBase : TestBase
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    protected UiTestBase(BrowserKind browserKind)
    {
        BrowserKind = browserKind;
    }

    public BrowserKind BrowserKind { get; }

    public IBrowserContext Context => _context ?? throw new InvalidOperationException("Context not initialised.");

    public IPage Page => _page ?? throw new InvalidOperationException("Page not initialised.");

    protected override async Task OnSetUpAsync()
    {
        var browserFactory = Services.GetRequiredService<BrowserFactory>();
        var contextFactory = Services.GetRequiredService<BrowserContextFactory>();
        var storage = Services.GetRequiredService<StorageStateManager>();
        var settings = Services.GetRequiredService<FrameworkSettings>();

        _browser = await browserFactory.LaunchAsync(BrowserKind).ConfigureAwait(false);

        var role = ResolveStorageStateRole();
        var storagePath = role is not null && storage.Exists(role) ? storage.PathFor(role) : null;

        var videoDir = PathHelper.EnsureDirectory(Path.Combine(ExecutionContext.ArtifactsDirectory, "video"));
        var harPath = Path.Combine(ExecutionContext.ArtifactsDirectory, "network.har");

        _context = await contextFactory.CreateAsync(_browser, new BrowserContextOptions
        {
            StorageStatePath = storagePath,
            VideoDirectory = videoDir,
            HarPath = harPath,
        }).ConfigureAwait(false);

        _page = await _context.NewPageAsync().ConfigureAwait(false);

        ExecutionContext.Items["page"] = _page;
        ExecutionContext.Items["context"] = _context;
        ExecutionContext.Items["browserName"] = BrowserKind.ToString();
        ExecutionContext.Items["videoDir"] = videoDir;
        ExecutionContext.Items["harPath"] = harPath;
    }

    protected override async Task OnTearDownAsync()
    {
        try
        {
            if (_context is not null && ExecutionContext.Outcome == TestOutcome.Failed)
            {
                var settings = Services.GetRequiredService<FrameworkSettings>();
                if (settings.Browser.RecordTrace)
                {
                    var tracePath = Path.Combine(ExecutionContext.ArtifactsDirectory, "trace.zip");
                    try
                    {
                        await _context.Tracing.StopAsync(new() { Path = tracePath }).ConfigureAwait(false);
                        ExecutionContext.Items["tracePath"] = tracePath;
                    }
                    catch (PlaywrightException)
                    {
                        // Tracing wasn't started (RecordTraceOnFailureOnly true) - acceptable.
                    }
                }

                if (settings.Browser.ScreenshotOnFailure && _page is not null)
                {
                    var shot = Path.Combine(ExecutionContext.ArtifactsDirectory, "failure.png");
                    await _page.ScreenshotAsync(new() { Path = shot, FullPage = true }).ConfigureAwait(false);
                    ExecutionContext.Items["screenshotPath"] = shot;
                }
            }
        }
        finally
        {
            if (_context is not null)
            {
                await _context.CloseAsync().ConfigureAwait(false);
            }
            if (_browser is not null)
            {
                await _browser.CloseAsync().ConfigureAwait(false);
            }
        }
    }

    private string? ResolveStorageStateRole()
    {
        var attr = (UseStorageStateAttribute?)Attribute.GetCustomAttribute(GetType(), typeof(UseStorageStateAttribute))
            ?? FindMethodAttribute();
        return attr?.Role;
    }

    private UseStorageStateAttribute? FindMethodAttribute()
    {
        var name = TestContext.CurrentContext.Test.MethodName;
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        var method = GetType().GetMethod(name);
        return method is null ? null : (UseStorageStateAttribute?)Attribute.GetCustomAttribute(method, typeof(UseStorageStateAttribute));
    }
}
