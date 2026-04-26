using System.Net.Http.Json;
using Framework.Configuration.Models;
using Microsoft.Extensions.Logging;

namespace Framework.Reporting.Notifications;

/// <summary>Posts a summary to a Slack incoming webhook.</summary>
public sealed class SlackNotifier : INotifier
{
    private readonly FrameworkSettings _settings;
    private readonly ILogger<SlackNotifier> _logger;
    private readonly HttpClient _http;

    public SlackNotifier(FrameworkSettings settings, ILogger<SlackNotifier> logger)
    {
        _settings = settings;
        _logger = logger;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task SendAsync(TestRunSummary summary, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Notifications.WebhookUrl))
        {
            _logger.LogDebug("Slack webhook not configured; skipping notification");
            return;
        }
        if (_settings.Notifications.OnlyOnFailure && summary.Failed == 0)
        {
            return;
        }

        var emoji = summary.Failed == 0 ? ":white_check_mark:" : ":x:";
        var payload = new
        {
            text = $"{emoji} *Test Run* ({summary.Environment}) - " +
                   $"Total: {summary.Total} | Passed: {summary.Passed} | Failed: {summary.Failed} | " +
                   $"Skipped: {summary.Skipped} | Duration: {summary.Duration:hh\\:mm\\:ss}\n" +
                   (string.IsNullOrEmpty(summary.ReportUrl) ? string.Empty : $"<{summary.ReportUrl}|Open report>"),
        };

        try
        {
            var resp = await _http.PostAsJsonAsync(_settings.Notifications.WebhookUrl, payload, ct).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Slack notification failed");
        }
    }
}
