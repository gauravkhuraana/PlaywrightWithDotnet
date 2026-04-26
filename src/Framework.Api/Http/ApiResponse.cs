using System.Text.Json;
using Framework.Utilities;

namespace Framework.Api.Http;

/// <summary>Typed wrapper around an API response.</summary>
public sealed class ApiResponse
{
    public required int StatusCode { get; init; }
    public required string StatusText { get; init; }
    public required IReadOnlyDictionary<string, string> Headers { get; init; }
    public required string Body { get; init; }
    public required TimeSpan Duration { get; init; }
    public required string RequestUrl { get; init; }
    public string? CorrelationId { get; init; }

    public bool IsSuccess => StatusCode is >= 200 and < 300;

    public T? As<T>() => string.IsNullOrEmpty(Body)
        ? default
        : JsonSerializer.Deserialize<T>(Body, JsonDefaults.Options);

    public JsonElement AsJsonElement()
    {
        using var doc = JsonDocument.Parse(Body);
        return doc.RootElement.Clone();
    }
}
