using Framework.Api.Http;
using Framework.Core.Hooks;
using Framework.Core.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Api.Testing;

/// <summary>Base fixture for API-only tests; exposes a configured <see cref="ApiClient"/>.</summary>
public abstract class ApiTestBase : TestBase
{
    private ApiClient? _client;

    public ApiClient Api => _client ?? throw new InvalidOperationException("ApiClient not initialised.");

    protected override async Task OnSetUpAsync()
    {
        var factory = Services.GetRequiredService<ApiClientFactory>();
        _client = await factory.CreateAsync().ConfigureAwait(false);
        ExecutionContext.Items["apiClient"] = _client;
    }

    protected override async Task OnTearDownAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync().ConfigureAwait(false);
        }
    }
}
