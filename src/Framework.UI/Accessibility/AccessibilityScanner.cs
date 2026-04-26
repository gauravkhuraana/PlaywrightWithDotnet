using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Framework.Configuration.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Framework.UI.Accessibility;

/// <summary>Wraps Axe-core scans against a Playwright page.</summary>
public sealed class AccessibilityScanner
{
    private readonly FrameworkSettings _settings;
    private readonly ILogger<AccessibilityScanner> _logger;

    public AccessibilityScanner(FrameworkSettings settings, ILogger<AccessibilityScanner> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task<AxeResult> ScanAsync(IPage page)
    {
        var runOptions = new AxeRunOptions
        {
            RunOnly = new RunOnlyOptions
            {
                Type = "tag",
                Values = _settings.Accessibility.Tags.ToList(),
            },
        };

        _logger.LogInformation("Running Axe accessibility scan");
        return await page.RunAxe(runOptions).ConfigureAwait(false);
    }

    /// <summary>Returns the violations whose impact meets or exceeds <see cref="AccessibilitySettings.MinimumImpact"/>.</summary>
    public IReadOnlyList<AxeResultItem> GetSignificantViolations(AxeResult result)
    {
        var threshold = ImpactRank(_settings.Accessibility.MinimumImpact);
        return result.Violations
            .Where(v => ImpactRank(v.Impact ?? "minor") >= threshold)
            .ToArray();
    }

    private static int ImpactRank(string impact) => impact.ToLowerInvariant() switch
    {
        "minor" => 1,
        "moderate" => 2,
        "serious" => 3,
        "critical" => 4,
        _ => 0,
    };
}
