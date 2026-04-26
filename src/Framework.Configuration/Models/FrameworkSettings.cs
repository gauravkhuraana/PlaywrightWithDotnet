namespace Framework.Configuration.Models;

/// <summary>
/// Root strongly-typed framework settings binding the entire <c>appsettings.json</c> tree.
/// </summary>
public sealed class FrameworkSettings
{
    public const string SectionName = "Framework";

    /// <summary>Logical environment name (e.g. dev, qa, staging, prod).</summary>
    public string Environment { get; set; } = "qa";

    /// <summary>Default test category run when none is supplied.</summary>
    public string DefaultCategory { get; set; } = "Smoke";

    public UiSettings Ui { get; set; } = new();
    public ApiSettings Api { get; set; } = new();
    public BrowserSettings Browser { get; set; } = new();
    public ReportingSettings Reporting { get; set; } = new();
    public RetrySettings Retry { get; set; } = new();
    public DatabaseSettings Database { get; set; } = new();
    public NotificationSettings Notifications { get; set; } = new();
    public VisualSettings Visual { get; set; } = new();
    public AccessibilitySettings Accessibility { get; set; } = new();
    public PerformanceSettings Performance { get; set; } = new();
    public KeyVaultSettings KeyVault { get; set; } = new();
}

public sealed class UiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int DefaultTimeoutMs { get; set; } = 30_000;
    public int NavigationTimeoutMs { get; set; } = 60_000;
    public bool Headless { get; set; } = true;
    public bool SlowMo { get; set; }
    public int SlowMoMs { get; set; }
    public string Locale { get; set; } = "en-US";
    public string TimezoneId { get; set; } = "UTC";
    public ViewportSettings Viewport { get; set; } = new();
}

public sealed class ViewportSettings
{
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
}

public sealed class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutMs { get; set; } = 30_000;
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
    public string? AuthScheme { get; set; }
    public string? AuthToken { get; set; }
}

public sealed class BrowserSettings
{
    /// <summary>Browsers enabled for execution (Chromium, Firefox, Webkit, Edge, Chrome).</summary>
    public string[] Enabled { get; set; } = { "Chromium" };
    public bool RecordVideo { get; set; }
    public bool RecordVideoOnFailureOnly { get; set; } = true;
    public bool RecordTrace { get; set; } = true;
    public bool RecordTraceOnFailureOnly { get; set; } = true;
    public bool ScreenshotOnFailure { get; set; } = true;
    public bool RecordHar { get; set; }
    public string[] MobileProfiles { get; set; } = Array.Empty<string>();
}

public sealed class ReportingSettings
{
    public string AllureResultsDirectory { get; set; } = "allure-results";
    public bool AttachTrace { get; set; } = true;
    public bool AttachVideo { get; set; } = true;
    public bool AttachScreenshot { get; set; } = true;
    public bool AttachHar { get; set; } = true;
    public bool AttachConsoleLog { get; set; } = true;
}

public sealed class RetrySettings
{
    public int MaxRetries { get; set; } = 1;
    public int ApiMaxRetries { get; set; } = 3;
    public int ApiBaseDelayMs { get; set; } = 500;
}

public sealed class DatabaseSettings
{
    public string Provider { get; set; } = "SqlServer"; // SqlServer | Postgres
    public string? ConnectionString { get; set; }
}

public sealed class NotificationSettings
{
    public string Channel { get; set; } = "None"; // None | Slack | Teams
    public string? WebhookUrl { get; set; }
    public bool OnlyOnFailure { get; set; } = true;
}

public sealed class VisualSettings
{
    public string BaselinesDirectory { get; set; } = "tests/.baselines";
    public string ActualDirectory { get; set; } = "tests/.visual/actual";
    public string DiffDirectory { get; set; } = "tests/.visual/diff";
    public double PixelTolerance { get; set; } = 0.1;
    public bool UpdateBaselines { get; set; }
}

public sealed class AccessibilitySettings
{
    public bool Enabled { get; set; } = true;
    public string MinimumImpact { get; set; } = "serious"; // minor | moderate | serious | critical
    public string[] Tags { get; set; } = { "wcag2a", "wcag2aa" };
}

public sealed class PerformanceSettings
{
    public bool Enabled { get; set; }
    public double MaxLcpMs { get; set; } = 2_500;
    public double MaxClsScore { get; set; } = 0.1;
    public double MaxTtfbMs { get; set; } = 800;
}

public sealed class KeyVaultSettings
{
    public bool Enabled { get; set; }
    public string? VaultUri { get; set; }
}
