using Framework.Configuration.Models;
using Framework.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Framework.UI.Visual;

/// <summary>Outcome of a visual comparison.</summary>
public sealed class VisualResult
{
    public bool Matches { get; init; }
    public double DiffRatio { get; init; }
    public string BaselinePath { get; init; } = string.Empty;
    public string ActualPath { get; init; } = string.Empty;
    public string? DiffPath { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Pixel-comparison visual regression helper. Stores baselines per browser per test name.
/// In <c>UpdateBaselines</c> mode the captured screenshot becomes the new baseline.
/// </summary>
public sealed class VisualVerifier
{
    private readonly FrameworkSettings _settings;
    private readonly ILogger<VisualVerifier> _logger;

    public VisualVerifier(FrameworkSettings settings, ILogger<VisualVerifier> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task<VisualResult> VerifyAsync(IPage page, string testName, string browserName, ILocator? scope = null)
    {
        var safeBrowser = PathHelper.SafeFileName(browserName);
        var safeTest = PathHelper.SafeFileName(testName);

        var baselineDir = PathHelper.EnsureDirectory(Path.Combine(_settings.Visual.BaselinesDirectory, safeBrowser));
        var actualDir = PathHelper.EnsureDirectory(Path.Combine(_settings.Visual.ActualDirectory, safeBrowser));
        var diffDir = PathHelper.EnsureDirectory(Path.Combine(_settings.Visual.DiffDirectory, safeBrowser));

        var baseline = Path.Combine(baselineDir, safeTest + ".png");
        var actual = Path.Combine(actualDir, safeTest + ".png");
        var diff = Path.Combine(diffDir, safeTest + ".png");

        var bytes = scope is not null
            ? await scope.ScreenshotAsync().ConfigureAwait(false)
            : await page.ScreenshotAsync(new() { FullPage = true }).ConfigureAwait(false);
        await File.WriteAllBytesAsync(actual, bytes).ConfigureAwait(false);

        if (!File.Exists(baseline) || _settings.Visual.UpdateBaselines)
        {
            await File.WriteAllBytesAsync(baseline, bytes).ConfigureAwait(false);
            _logger.LogInformation("Visual baseline {Mode} for {Test}", _settings.Visual.UpdateBaselines ? "updated" : "created", testName);
            return new VisualResult
            {
                Matches = true,
                BaselinePath = baseline,
                ActualPath = actual,
                Message = "Baseline created/updated",
            };
        }

        using var b = await Image.LoadAsync<Rgba32>(baseline).ConfigureAwait(false);
        using var a = await Image.LoadAsync<Rgba32>(actual).ConfigureAwait(false);

        if (b.Width != a.Width || b.Height != a.Height)
        {
            return new VisualResult
            {
                Matches = false,
                BaselinePath = baseline,
                ActualPath = actual,
                Message = $"Size mismatch: baseline {b.Width}x{b.Height} actual {a.Width}x{a.Height}",
            };
        }

        var (ratio, diffImage) = CompareAndProduceDiff(b, a);
        await diffImage.SaveAsPngAsync(diff).ConfigureAwait(false);
        diffImage.Dispose();

        var matches = ratio <= _settings.Visual.PixelTolerance;
        return new VisualResult
        {
            Matches = matches,
            DiffRatio = ratio,
            BaselinePath = baseline,
            ActualPath = actual,
            DiffPath = diff,
            Message = matches
                ? $"Within tolerance ({ratio:P3} <= {_settings.Visual.PixelTolerance:P3})"
                : $"Diff {ratio:P3} exceeds tolerance {_settings.Visual.PixelTolerance:P3}",
        };
    }

    private static (double Ratio, Image<Rgba32> DiffImage) CompareAndProduceDiff(Image<Rgba32> baseline, Image<Rgba32> actual)
    {
        var diff = new Image<Rgba32>(baseline.Width, baseline.Height);
        long different = 0;
        long total = (long)baseline.Width * baseline.Height;

        for (var y = 0; y < baseline.Height; y++)
        {
            for (var x = 0; x < baseline.Width; x++)
            {
                var p1 = baseline[x, y];
                var p2 = actual[x, y];
                if (PixelDistance(p1, p2) > 32)
                {
                    diff[x, y] = new Rgba32(255, 0, 0, 255);
                    different++;
                }
                else
                {
                    diff[x, y] = new Rgba32(p1.R, p1.G, p1.B, 80);
                }
            }
        }

        return ((double)different / total, diff);
    }

    private static int PixelDistance(Rgba32 a, Rgba32 b)
    {
        return Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);
    }
}
