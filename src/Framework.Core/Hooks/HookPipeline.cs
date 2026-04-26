using Microsoft.Extensions.Logging;

namespace Framework.Core.Hooks;

/// <summary>Runs registered <see cref="ITestHook"/> instances in order.</summary>
public sealed class HookPipeline
{
    private readonly IReadOnlyList<ITestHook> _hooks;
    private readonly ILogger<HookPipeline> _logger;

    public HookPipeline(IEnumerable<ITestHook> hooks, ILogger<HookPipeline> logger)
    {
        _hooks = hooks.OrderBy(h => h.Order).ToArray();
        _logger = logger;
    }

    public async Task BeforeAsync(TestExecutionContext context, CancellationToken ct = default)
    {
        foreach (var hook in _hooks)
        {
            try
            {
                await hook.BeforeAsync(context, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hook {Hook} failed in BeforeAsync", hook.GetType().Name);
                throw;
            }
        }
    }

    public async Task AfterAsync(TestExecutionContext context, CancellationToken ct = default)
    {
        foreach (var hook in _hooks.Reverse())
        {
            try
            {
                await hook.AfterAsync(context, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // After hooks must not mask the original test failure.
                _logger.LogError(ex, "Hook {Hook} failed in AfterAsync", hook.GetType().Name);
            }
        }
    }
}
