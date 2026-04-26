using Framework.Core.DependencyInjection;
using Framework.UI.Accessibility;
using Framework.UI.Browser;
using Framework.UI.Performance;
using Framework.UI.Visual;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.UI;

/// <summary>Registers the UI module services into the framework DI container.</summary>
public static class FrameworkUiModule
{
    private static int _registered;

    /// <summary>Idempotently registers all UI services. Call from a module initializer or test bootstrap.</summary>
    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        FrameworkServiceRegistration.AddRegistration((services, _) =>
        {
            services.AddSingleton<BrowserFactory>();
            services.AddSingleton<BrowserContextFactory>();
            services.AddSingleton<StorageStateManager>();
            services.AddSingleton<VisualVerifier>();
            services.AddSingleton<AccessibilityScanner>();
            services.AddSingleton<PerformanceProbe>();
        });
    }
}
