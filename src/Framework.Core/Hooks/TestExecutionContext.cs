namespace Framework.Core.Hooks;

/// <summary>Test outcome for the hook pipeline.</summary>
public enum TestOutcome
{
    Unknown,
    Passed,
    Failed,
    Skipped,
}

/// <summary>
/// Per-test execution context flowing through the hook pipeline.
/// Hooks may stash arbitrary objects via <see cref="Items"/>.
/// </summary>
public sealed class TestExecutionContext
{
    public required string TestName { get; init; }

    public required string FullyQualifiedName { get; init; }

    public required string ClassName { get; init; }

    public required IServiceProvider Services { get; init; }

    public string? BrowserName { get; init; }

    public Dictionary<string, object?> Items { get; } = new(StringComparer.OrdinalIgnoreCase);

    public TestOutcome Outcome { get; set; } = TestOutcome.Unknown;

    public Exception? Exception { get; set; }

    public string ArtifactsDirectory { get; set; } = string.Empty;
}
