using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Framework.UI.Pages;

/// <summary>
/// Base for reusable UI components scoped to a root <see cref="ILocator"/>
/// (header, modal dialog, data grid, form, etc.).
/// </summary>
public abstract class ComponentBase
{
    protected ComponentBase(ILocator root, IPage page, ILogger logger)
    {
        Root = root;
        Page = page;
        Logger = logger;
    }

    public ILocator Root { get; }

    public IPage Page { get; }

    protected ILogger Logger { get; }

    public virtual Task<bool> IsVisibleAsync() => Root.IsVisibleAsync();
}
