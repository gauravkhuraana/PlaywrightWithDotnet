using Framework.Api.Http;
using Framework.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Api;

/// <summary>Registers API services into the framework DI container.</summary>
public static class FrameworkApiModule
{
    private static int _registered;

    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        FrameworkServiceRegistration.AddRegistration((services, _) =>
        {
            services.AddSingleton<ApiClientFactory>();
        });
    }
}
