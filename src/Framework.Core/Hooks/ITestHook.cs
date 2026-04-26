namespace Framework.Core.Hooks;

/// <summary>
/// Lightweight chain-of-responsibility hook pipeline executed before/after every test.
/// Reporting, screenshot-on-failure, trace start/stop, etc. plug in here.
/// </summary>
public interface ITestHook
{
    /// <summary>Order ascending; lower runs first in <c>Before</c>, last in <c>After</c>.</summary>
    int Order { get; }

    Task BeforeAsync(TestExecutionContext context, CancellationToken cancellationToken);

    Task AfterAsync(TestExecutionContext context, CancellationToken cancellationToken);
}
