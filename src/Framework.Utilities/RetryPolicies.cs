using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Framework.Utilities;

/// <summary>Factory for common Polly resilience pipelines.</summary>
public static class RetryPolicies
{
    /// <summary>Exponential-backoff retry pipeline for transient HTTP-like failures.</summary>
    public static ResiliencePipeline ExponentialBackoff(int maxRetries, int baseDelayMs, ILogger? logger = null)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = maxRetries,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(baseDelayMs),
                UseJitter = true,
                ShouldHandle = new PredicateBuilder().Handle<Exception>(ex => ex is not OperationCanceledException),
                OnRetry = args =>
                {
                    logger?.LogWarning(
                        args.Outcome.Exception,
                        "Retry {Attempt}/{Max} after {Delay}ms",
                        args.AttemptNumber + 1,
                        maxRetries,
                        args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                },
            })
            .Build();
    }
}
