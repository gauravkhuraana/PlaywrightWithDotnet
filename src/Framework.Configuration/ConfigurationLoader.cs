using Azure.Identity;
using Framework.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace Framework.Configuration;

/// <summary>
/// Loads layered configuration:
/// 1. <c>appsettings.json</c> base
/// 2. <c>appsettings.{TEST_ENV}.json</c> environment overlay
/// 3. <c>appsettings.Local.json</c> developer-only overrides
/// 4. Environment variables (prefix <c>PWFX_</c>)
/// 5. Optional Azure Key Vault secrets when <c>Framework:KeyVault:Enabled</c> is true.
/// </summary>
public static class ConfigurationLoader
{
    public const string EnvironmentVariableName = "TEST_ENV";
    public const string EnvVarPrefix = "PWFX_";

    /// <summary>Builds an <see cref="IConfigurationRoot"/> using the layered providers.</summary>
    /// <param name="basePath">Base path; defaults to current directory.</param>
    /// <param name="environment">Environment override; otherwise read from <c>TEST_ENV</c>.</param>
    public static IConfigurationRoot Build(string? basePath = null, string? environment = null)
    {
        basePath ??= Directory.GetCurrentDirectory();
        environment ??= Environment.GetEnvironmentVariable(EnvironmentVariableName) ?? "qa";

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: EnvVarPrefix);

        // First pass to resolve Key Vault settings if enabled.
        var preliminary = builder.Build();
        var kv = new KeyVaultSettings();
        preliminary.GetSection($"{FrameworkSettings.SectionName}:KeyVault").Bind(kv);

        if (kv.Enabled && !string.IsNullOrWhiteSpace(kv.VaultUri))
        {
            builder.AddAzureKeyVault(new Uri(kv.VaultUri!), new DefaultAzureCredential());
        }

        // Force environment into the resolved settings so downstream consumers see it.
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{FrameworkSettings.SectionName}:Environment"] = environment,
        });

        return builder.Build();
    }

    /// <summary>Binds <see cref="FrameworkSettings"/> from the supplied configuration root.</summary>
    public static FrameworkSettings BindSettings(IConfiguration configuration)
    {
        var settings = new FrameworkSettings();
        configuration.GetSection(FrameworkSettings.SectionName).Bind(settings);
        return settings;
    }
}
