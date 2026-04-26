using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Framework.Configuration.Models;
using Framework.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Polly;

namespace Framework.Api.Http;

/// <summary>
/// API client wrapping <see cref="IAPIRequestContext"/>.
/// Provides typed request/response, default headers, auth handling, retries, and request logging.
/// </summary>
public sealed class ApiClient : IAsyncDisposable
{
    private readonly IAPIRequestContext _context;
    private readonly FrameworkSettings _settings;
    private readonly ILogger<ApiClient> _logger;
    private readonly ResiliencePipeline _retry;

    internal ApiClient(IAPIRequestContext context, FrameworkSettings settings, ILogger<ApiClient> logger)
    {
        _context = context;
        _settings = settings;
        _logger = logger;
        _retry = RetryPolicies.ExponentialBackoff(
            settings.Retry.ApiMaxRetries,
            settings.Retry.ApiBaseDelayMs,
            logger);
    }

    public async Task<ApiResponse> SendAsync(ApiRequest request, CancellationToken ct = default)
    {
        var url = BuildUrl(request);
        var headers = MergeHeaders(request);
        var correlationId = request.CorrelationId ?? Guid.NewGuid().ToString("N");
        headers["X-Correlation-Id"] = correlationId;

        var body = SerializeBody(request, headers);

        var sw = Stopwatch.StartNew();
        var resp = await _retry.ExecuteAsync(async token =>
        {
            _logger.LogInformation("[{CorrelationId}] {Verb} {Url}", correlationId, request.Verb, url);
            return await DispatchAsync(request, url, headers, body).ConfigureAwait(false);
        }, ct).ConfigureAwait(false);
        sw.Stop();

        var responseBody = await resp.TextAsync().ConfigureAwait(false);
        var responseHeaders = resp.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

        _logger.LogInformation(
            "[{CorrelationId}] -> {Status} {StatusText} in {Duration}ms",
            correlationId, resp.Status, resp.StatusText, sw.ElapsedMilliseconds);

        return new ApiResponse
        {
            StatusCode = resp.Status,
            StatusText = resp.StatusText,
            Headers = responseHeaders,
            Body = responseBody,
            Duration = sw.Elapsed,
            RequestUrl = url,
            CorrelationId = correlationId,
        };
    }

    private async Task<IAPIResponse> DispatchAsync(
        ApiRequest request,
        string url,
        Dictionary<string, string> headers,
        string? body)
    {
        var options = new APIRequestContextOptions
        {
            Headers = headers,
            Timeout = (float?)(request.Timeout?.TotalMilliseconds ?? _settings.Api.TimeoutMs),
            DataString = body,
        };

        return request.Verb switch
        {
            HttpVerb.Get => await _context.GetAsync(url, options).ConfigureAwait(false),
            HttpVerb.Post => await _context.PostAsync(url, options).ConfigureAwait(false),
            HttpVerb.Put => await _context.PutAsync(url, options).ConfigureAwait(false),
            HttpVerb.Patch => await _context.PatchAsync(url, options).ConfigureAwait(false),
            HttpVerb.Delete => await _context.DeleteAsync(url, options).ConfigureAwait(false),
            HttpVerb.Head => await _context.HeadAsync(url, options).ConfigureAwait(false),
            HttpVerb.Options => await _context.FetchAsync(url, new APIRequestContextOptions
            {
                Method = "OPTIONS",
                Headers = headers,
                DataString = body,
            }).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(request)),
        };
    }

    private string BuildUrl(ApiRequest request)
    {
        var path = request.Path.StartsWith('/') ? request.Path : "/" + request.Path;
        var baseUrl = (_settings.Api.BaseUrl ?? string.Empty).TrimEnd('/');
        var url = baseUrl + path;
        if (request.QueryParams.Count == 0)
        {
            return url;
        }

        var qs = string.Join("&", request.QueryParams.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        return url + (url.Contains('?') ? "&" : "?") + qs;
    }

    private Dictionary<string, string> MergeHeaders(ApiRequest request)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in _settings.Api.DefaultHeaders)
        {
            headers[kvp.Key] = kvp.Value;
        }
        foreach (var kvp in request.Headers)
        {
            headers[kvp.Key] = kvp.Value;
        }

        if (!string.IsNullOrEmpty(_settings.Api.AuthScheme) && !string.IsNullOrEmpty(_settings.Api.AuthToken))
        {
            headers["Authorization"] = $"{_settings.Api.AuthScheme} {_settings.Api.AuthToken}";
        }
        return headers;
    }

    private static string? SerializeBody(ApiRequest request, Dictionary<string, string> headers)
    {
        if (!string.IsNullOrEmpty(request.RawBody))
        {
            headers.TryAdd("Content-Type", request.ContentType ?? "application/json");
            return request.RawBody;
        }
        if (request.Body is null)
        {
            return null;
        }
        headers.TryAdd("Content-Type", request.ContentType ?? "application/json");
        return JsonSerializer.Serialize(request.Body, JsonDefaults.Options);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync().ConfigureAwait(false);
    }
}
