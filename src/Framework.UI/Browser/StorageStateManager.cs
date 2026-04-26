using Framework.Configuration.Models;
using Framework.Utilities;

namespace Framework.UI.Browser;

/// <summary>
/// Manages persisted Playwright <c>storageState</c> files used to skip repeated logins.
/// </summary>
public sealed class StorageStateManager
{
    private readonly FrameworkSettings _settings;
    private readonly string _root;

    public StorageStateManager(FrameworkSettings settings)
    {
        _settings = settings;
        _root = PathHelper.EnsureDirectory(Path.Combine("auth"));
    }

    /// <summary>Path for the supplied role under the active environment.</summary>
    public string PathFor(string role)
    {
        var safeRole = PathHelper.SafeFileName(role);
        var safeEnv = PathHelper.SafeFileName(_settings.Environment);
        return Path.Combine(_root, $"{safeEnv}-{safeRole}.json");
    }

    public bool Exists(string role) => File.Exists(PathFor(role));

    public void Delete(string role)
    {
        var p = PathFor(role);
        if (File.Exists(p))
        {
            File.Delete(p);
        }
    }
}
