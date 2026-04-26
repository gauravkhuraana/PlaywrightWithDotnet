using Framework.Configuration.Models;
using Framework.Core.DependencyInjection;
using Framework.Core.Hooks;
using Framework.Reporting.Allure;
using Framework.Reporting.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Reporting;

/// <summary>Registers reporting services into the DI container.</summary>
public static class FrameworkReportingModule
{
    private static int _registered;

    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        FrameworkServiceRegistration.AddRegistration((services, settings) =>
        {
            services.AddSingleton<ITestHook, AllureAttachmentsHook>();

            switch (settings.Notifications.Channel.ToLowerInvariant())
            {
                case "slack":
                    services.AddSingleton<INotifier, SlackNotifier>();
                    break;
                case "teams":
                    services.AddSingleton<INotifier, TeamsNotifier>();
                    break;
                default:
                    services.AddSingleton<INotifier, NoOpNotifier>();
                    break;
            }
        });
    }
}
