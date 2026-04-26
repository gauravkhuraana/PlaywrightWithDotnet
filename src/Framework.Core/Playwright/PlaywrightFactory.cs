using Microsoft.Playwright;

namespace Framework.Core.Playwright;

/// <summary>Process-wide singleton wrapper around <see cref="IPlaywright"/>.</summary>
public sealed class PlaywrightFactory : IAsyncDisposable
{
    private static readonly SemaphoreSlim _gate = new(1, 1);
    private IPlaywright? _playwright;

    public async Task<IPlaywright> GetAsync()
    {
        if (_playwright is not null)
        {
            return _playwright;
        }

        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            _playwright ??= await Microsoft.Playwright.Playwright.CreateAsync().ConfigureAwait(false);
            return _playwright;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _playwright?.Dispose();
        _playwright = null;
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
