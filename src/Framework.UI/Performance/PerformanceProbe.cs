using Framework.Configuration.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Framework.UI.Performance;

/// <summary>Captured Web Vitals snapshot.</summary>
public sealed record WebVitals(double Lcp, double Cls, double Ttfb, double DomContentLoaded, double Load);

/// <summary>
/// Captures basic Web Vitals via in-page <c>PerformanceObserver</c> probes.
/// Suitable for smoke-level perf assertions, not a replacement for Lighthouse.
/// </summary>
public sealed class PerformanceProbe
{
    private readonly FrameworkSettings _settings;
    private readonly ILogger<PerformanceProbe> _logger;

    public PerformanceProbe(FrameworkSettings settings, ILogger<PerformanceProbe> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task<WebVitals> CaptureAsync(IPage page)
    {
        const string script = """
            () => new Promise(resolve => {
                let lcp = 0, cls = 0;
                try {
                    new PerformanceObserver(list => {
                        for (const e of list.getEntries()) lcp = Math.max(lcp, e.startTime);
                    }).observe({ type: 'largest-contentful-paint', buffered: true });
                    new PerformanceObserver(list => {
                        for (const e of list.getEntries()) if (!e.hadRecentInput) cls += e.value;
                    }).observe({ type: 'layout-shift', buffered: true });
                } catch { }
                setTimeout(() => {
                    const nav = performance.getEntriesByType('navigation')[0] || {};
                    resolve({
                        lcp,
                        cls,
                        ttfb: nav.responseStart || 0,
                        dcl: nav.domContentLoadedEventEnd || 0,
                        load: nav.loadEventEnd || 0,
                    });
                }, 1000);
            })
            """;

        var json = await page.EvaluateAsync<System.Text.Json.JsonElement>(script).ConfigureAwait(false);
        var vitals = new WebVitals(
            Lcp: json.GetProperty("lcp").GetDouble(),
            Cls: json.GetProperty("cls").GetDouble(),
            Ttfb: json.GetProperty("ttfb").GetDouble(),
            DomContentLoaded: json.GetProperty("dcl").GetDouble(),
            Load: json.GetProperty("load").GetDouble());

        _logger.LogInformation(
            "Web Vitals: LCP={Lcp:F0}ms CLS={Cls:F3} TTFB={Ttfb:F0}ms DCL={Dcl:F0}ms Load={Load:F0}ms",
            vitals.Lcp, vitals.Cls, vitals.Ttfb, vitals.DomContentLoaded, vitals.Load);
        return vitals;
    }

    public IReadOnlyList<string> EvaluateThresholds(WebVitals vitals)
    {
        var failures = new List<string>();
        if (!_settings.Performance.Enabled)
        {
            return failures;
        }

        if (vitals.Lcp > _settings.Performance.MaxLcpMs)
        {
            failures.Add($"LCP {vitals.Lcp:F0}ms > {_settings.Performance.MaxLcpMs:F0}ms");
        }
        if (vitals.Cls > _settings.Performance.MaxClsScore)
        {
            failures.Add($"CLS {vitals.Cls:F3} > {_settings.Performance.MaxClsScore:F3}");
        }
        if (vitals.Ttfb > _settings.Performance.MaxTtfbMs)
        {
            failures.Add($"TTFB {vitals.Ttfb:F0}ms > {_settings.Performance.MaxTtfbMs:F0}ms");
        }

        return failures;
    }
}
