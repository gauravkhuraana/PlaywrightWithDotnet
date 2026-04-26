namespace Framework.Reporting.Notifications;

/// <summary>Summary published to a notifier at end-of-run.</summary>
public sealed record TestRunSummary(
    int Total,
    int Passed,
    int Failed,
    int Skipped,
    TimeSpan Duration,
    string Environment,
    string ReportUrl);

/// <summary>Strategy for posting end-of-run notifications.</summary>
public interface INotifier
{
    Task SendAsync(TestRunSummary summary, CancellationToken ct = default);
}

/// <summary>No-op notifier used when notifications are disabled.</summary>
public sealed class NoOpNotifier : INotifier
{
    public Task SendAsync(TestRunSummary summary, CancellationToken ct = default) => Task.CompletedTask;
}
