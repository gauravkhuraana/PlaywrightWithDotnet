using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Framework.UI.Pages;

/// <summary>
/// Base class for page objects. Provides logger, page accessor, and common navigation primitives.
/// Implementations expose typed locator properties and high-level actions.
/// </summary>
public abstract class PageBase
{
    protected PageBase(IPage page, ILogger logger)
    {
        Page = page;
        Logger = logger;
    }

    public IPage Page { get; }

    protected ILogger Logger { get; }

    /// <summary>The relative URL the page lives at; consumed by <see cref="GoToAsync"/>.</summary>
    public abstract string RelativeUrl { get; }

    /// <summary>Navigates to <see cref="RelativeUrl"/> using the context's <c>baseURL</c>.</summary>
    public virtual async Task GoToAsync()
    {
        Logger.LogInformation("Navigating to {Url}", RelativeUrl);
        await Page.GotoAsync(RelativeUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
        }).ConfigureAwait(false);
        await VerifyLoadedAsync().ConfigureAwait(false);
    }

    /// <summary>Override to assert this page has fully loaded (visible header, key element, etc.).</summary>
    protected virtual Task VerifyLoadedAsync() => Task.CompletedTask;
}
