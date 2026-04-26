using System.Net.Http.Json;
using Framework.Configuration.Models;
using Microsoft.Extensions.Logging;

namespace Framework.Reporting.Notifications;

/// <summary>Posts a summary to a Microsoft Teams incoming webhook (legacy connector format).</summary>
public sealed class TeamsNotifier : INotifier
{
    private readonly FrameworkSettings _settings;
    private readonly ILogger<TeamsNotifier> _logger;
    private readonly HttpClient _http;

    public TeamsNotifier(FrameworkSettings settings, ILogger<TeamsNotifier> logger)
    {
        _settings = settings;
        _logger = logger;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task SendAsync(TestRunSummary summary, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Notifications.WebhookUrl))
        {
            return;
        }
        if (_settings.Notifications.OnlyOnFailure && summary.Failed == 0)
        {
            return;
        }

        var color = summary.Failed == 0 ? "00FF00" : "FF0000";
        var payload = new
        {
            @type = "MessageCard",
            @context = "https://schema.org/extensions",
            themeColor = color,
            summary = "Test Run",
            title = $"Test Run ({summary.Environment})",
            text = $"Total: {summary.Total} | Passed: {summary.Passed} | Failed: {summary.Failed} | " +
                   $"Skipped: {summary.Skipped} | Duration: {summary.Duration:hh\\:mm\\:ss}" +
                   (string.IsNullOrEmpty(summary.ReportUrl) ? string.Empty : $"\n\n[Open report]({summary.ReportUrl})"),
        };

        try
        {
            var resp = await _http.PostAsJsonAsync(_settings.Notifications.WebhookUrl, payload, ct).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Teams notification failed");
        }
    }
}
