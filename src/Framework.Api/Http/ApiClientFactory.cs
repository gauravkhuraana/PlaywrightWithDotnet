using Framework.Configuration.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Framework.Api.Http;

/// <summary>Creates configured <see cref="ApiClient"/> instances per test.</summary>
public sealed class ApiClientFactory
{
    private readonly IPlaywright _playwright;
    private readonly FrameworkSettings _settings;
    private readonly ILoggerFactory _loggerFactory;

    public ApiClientFactory(IPlaywright playwright, FrameworkSettings settings, ILoggerFactory loggerFactory)
    {
        _playwright = playwright;
        _settings = settings;
        _loggerFactory = loggerFactory;
    }

    public async Task<ApiClient> CreateAsync()
    {
        var ctx = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = _settings.Api.BaseUrl,
            Timeout = _settings.Api.TimeoutMs,
            IgnoreHTTPSErrors = true,
        }).ConfigureAwait(false);

        return new ApiClient(ctx, _settings, _loggerFactory.CreateLogger<ApiClient>());
    }
}
