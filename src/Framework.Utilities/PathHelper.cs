namespace Framework.Utilities;

/// <summary>Filesystem helpers used across the framework.</summary>
public static class PathHelper
{
    /// <summary>Ensures the directory exists, creating it if needed; returns the full path.</summary>
    public static string EnsureDirectory(string path)
    {
        var full = Path.GetFullPath(path);
        Directory.CreateDirectory(full);
        return full;
    }

    /// <summary>Resolves a path relative to the test execution base directory.</summary>
    public static string ResolveFromBase(params string[] segments)
    {
        var combined = Path.Combine(new[] { AppContext.BaseDirectory }.Concat(segments).ToArray());
        return Path.GetFullPath(combined);
    }

    /// <summary>Sanitises a string for safe use as a file or directory name.</summary>
    public static string SafeFileName(string raw)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var clean = new string(raw.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return clean.Length > 120 ? clean[..120] : clean;
    }
}
