using Framework.Configuration;
using Framework.Configuration.Models;
using Framework.Core.Hooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace Framework.Core.DependencyInjection;

/// <summary>
/// Builds the framework-wide <see cref="IServiceProvider"/>.
/// One root provider exists per process; per-test scopes are created by <c>TestBase</c>.
/// </summary>
public static class FrameworkServiceProvider
{
    private static readonly object _gate = new();
    private static IServiceProvider? _root;

    public static IServiceProvider Root
    {
        get
        {
            if (_root is not null)
            {
                return _root;
            }

            lock (_gate)
            {
                _root ??= Build();
            }

            return _root!;
        }
    }

    /// <summary>Resets the singleton (intended for tests of the framework itself).</summary>
    public static void Reset()
    {
        lock (_gate)
        {
            (_root as IDisposable)?.Dispose();
            _root = null;
        }
    }

    private static IServiceProvider Build()
    {
        var config = ConfigurationLoader.Build();
        var settings = ConfigurationLoader.BindSettings(config);

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.Configure<FrameworkSettings>(config.GetSection(FrameworkSettings.SectionName));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<FrameworkSettings>>().Value);

        // Serilog -> Microsoft.Extensions.Logging
        var serilog = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .CreateLogger();
        Log.Logger = serilog;
        services.AddLogging(b => b.AddSerilog(serilog, dispose: true));

        // Always register the hook pipeline. Hooks themselves are added by feature projects.
        services.AddScoped<HookPipeline>();

        // Shared Playwright singleton consumable by both UI and API modules.
        services.AddSingleton<Playwright.PlaywrightFactory>();
        services.AddSingleton<Microsoft.Playwright.IPlaywright>(sp =>
            sp.GetRequiredService<Playwright.PlaywrightFactory>().GetAsync().GetAwaiter().GetResult());

        FrameworkServiceRegistration.Register(services, settings);
        return services.BuildServiceProvider(validateScopes: true);
    }
}

/// <summary>
/// Extension hook so feature projects (UI, Api, Reporting, Data) can register their own services
/// without <c>Framework.Core</c> taking project references on them.
/// </summary>
public static class FrameworkServiceRegistration
{
    private static readonly List<Action<IServiceCollection, FrameworkSettings>> _registrations = new();

    /// <summary>Adds a registration callback. Safe to call multiple times.</summary>
    public static void AddRegistration(Action<IServiceCollection, FrameworkSettings> registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        _registrations.Add(registration);
    }

    internal static void Register(IServiceCollection services, FrameworkSettings settings)
    {
        foreach (var reg in _registrations)
        {
            reg(services, settings);
        }
    }
}
