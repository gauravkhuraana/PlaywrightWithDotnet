using Framework.Configuration.Models;
using Framework.Core.DependencyInjection;
using Framework.Core.Hooks;
using Framework.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Framework.Core.Testing;

/// <summary>
/// Base class for all NUnit test fixtures.
/// Provides a per-test DI scope, logger, hook pipeline, and artifacts directory.
/// </summary>
[TestFixture]
public abstract class TestBase
{
    private IServiceScope? _scope;

    protected IServiceProvider Services => _scope?.ServiceProvider
        ?? throw new InvalidOperationException("Service scope not initialised. SetUpAsync must run first.");

    protected FrameworkSettings Settings => Services.GetRequiredService<FrameworkSettings>();

    protected ILogger Logger { get; private set; } = default!;

    protected TestExecutionContext ExecutionContext { get; private set; } = default!;

    [SetUp]
    public async Task FrameworkSetUpAsync()
    {
        _scope = FrameworkServiceProvider.Root.CreateScope();

        var test = TestContext.CurrentContext.Test;
        var loggerFactory = Services.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger(test.ClassName ?? GetType().FullName!);

        var artifactsDir = PathHelper.EnsureDirectory(Path.Combine(
            "TestResults",
            "artifacts",
            PathHelper.SafeFileName(test.ClassName ?? GetType().Name),
            PathHelper.SafeFileName(test.Name)));

        ExecutionContext = new TestExecutionContext
        {
            TestName = test.Name,
            FullyQualifiedName = test.FullName,
            ClassName = test.ClassName ?? GetType().FullName!,
            Services = Services,
            ArtifactsDirectory = artifactsDir,
        };

        Logger.LogInformation("--- BEGIN {Test} ---", ExecutionContext.FullyQualifiedName);

        var pipeline = Services.GetRequiredService<HookPipeline>();
        await pipeline.BeforeAsync(ExecutionContext).ConfigureAwait(false);

        await OnSetUpAsync().ConfigureAwait(false);
    }

    [TearDown]
    public async Task FrameworkTearDownAsync()
    {
        try
        {
            var result = TestContext.CurrentContext.Result;
            ExecutionContext.Outcome = result.Outcome.Status switch
            {
                TestStatus.Passed => TestOutcome.Passed,
                TestStatus.Failed => TestOutcome.Failed,
                TestStatus.Skipped => TestOutcome.Skipped,
                _ => TestOutcome.Unknown,
            };
            if (!string.IsNullOrEmpty(result.Message) && ExecutionContext.Outcome == TestOutcome.Failed)
            {
                ExecutionContext.Exception = new Exception(result.Message + Environment.NewLine + result.StackTrace);
            }

            await OnTearDownAsync().ConfigureAwait(false);

            var pipeline = Services.GetRequiredService<HookPipeline>();
            await pipeline.AfterAsync(ExecutionContext).ConfigureAwait(false);

            Logger.LogInformation("--- END {Test} [{Outcome}] ---", ExecutionContext.FullyQualifiedName, ExecutionContext.Outcome);
        }
        finally
        {
            _scope?.Dispose();
            _scope = null;
        }
    }

    /// <summary>Override for fixture-specific setup that runs after framework hooks.</summary>
    protected virtual Task OnSetUpAsync() => Task.CompletedTask;

    /// <summary>Override for fixture-specific teardown that runs before framework hooks.</summary>
    protected virtual Task OnTearDownAsync() => Task.CompletedTask;
}
