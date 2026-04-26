namespace Framework.UI.Browser;

/// <summary>Supported browser engines / channels.</summary>
public enum BrowserKind
{
    Chromium,
    Firefox,
    Webkit,
    Edge,
    Chrome,
}

public static class BrowserKindParser
{
    public static BrowserKind Parse(string value) => value.Trim().ToLowerInvariant() switch
    {
        "chromium" => BrowserKind.Chromium,
        "firefox" => BrowserKind.Firefox,
        "webkit" or "safari" => BrowserKind.Webkit,
        "edge" or "msedge" => BrowserKind.Edge,
        "chrome" or "google-chrome" => BrowserKind.Chrome,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported browser kind"),
    };
}
