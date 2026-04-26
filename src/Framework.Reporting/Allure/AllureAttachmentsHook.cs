using Allure.Net.Commons;
using Framework.Configuration.Models;
using Framework.Core.Hooks;
using Microsoft.Extensions.Logging;

namespace Framework.Reporting.Allure;

/// <summary>
/// Hook that attaches Playwright artefacts (screenshot, trace, video, HAR) to the Allure report
/// when a test fails.
/// </summary>
public sealed class AllureAttachmentsHook : ITestHook
{
    private readonly FrameworkSettings _settings;
    private readonly ILogger<AllureAttachmentsHook> _logger;

    public AllureAttachmentsHook(FrameworkSettings settings, ILogger<AllureAttachmentsHook> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public int Order => 100;

    public Task BeforeAsync(TestExecutionContext context, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task AfterAsync(TestExecutionContext context, CancellationToken cancellationToken)
    {
        if (context.Outcome != TestOutcome.Failed)
        {
            return Task.CompletedTask;
        }

        TryAttach(context, "screenshotPath", "Screenshot", "image/png", _settings.Reporting.AttachScreenshot);
        TryAttach(context, "tracePath", "Trace", "application/zip", _settings.Reporting.AttachTrace);
        TryAttach(context, "harPath", "HAR", "application/json", _settings.Reporting.AttachHar);
        TryAttachVideoDirectory(context);
        return Task.CompletedTask;
    }

    private void TryAttach(TestExecutionContext context, string itemKey, string label, string mime, bool enabled)
    {
        if (!enabled)
        {
            return;
        }
        if (!context.Items.TryGetValue(itemKey, out var path) || path is not string p || !File.Exists(p))
        {
            return;
        }

        try
        {
            AllureApi.AddAttachment(label, mime, p);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to attach {Label} to Allure", label);
        }
    }

    private void TryAttachVideoDirectory(TestExecutionContext context)
    {
        if (!_settings.Reporting.AttachVideo)
        {
            return;
        }
        if (!context.Items.TryGetValue("videoDir", out var path) || path is not string dir || !Directory.Exists(dir))
        {
            return;
        }

        foreach (var file in Directory.EnumerateFiles(dir, "*.webm"))
        {
            try
            {
                AllureApi.AddAttachment("Video", "video/webm", file);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to attach video {File}", file);
            }
        }
    }
}
