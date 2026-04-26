namespace Framework.Api.Http;

/// <summary>HTTP verbs supported by <see cref="ApiClient"/>.</summary>
public enum HttpVerb
{
    Get,
    Post,
    Put,
    Patch,
    Delete,
    Head,
    Options,
}

/// <summary>Fluent request builder.</summary>
public sealed class ApiRequest
{
    public HttpVerb Verb { get; init; } = HttpVerb.Get;
    public string Path { get; init; } = "/";
    public Dictionary<string, string> Headers { get; init; } = new();
    public Dictionary<string, string> QueryParams { get; init; } = new();
    public object? Body { get; init; }
    public string? RawBody { get; init; }
    public string? ContentType { get; init; }
    public string? CorrelationId { get; init; }
    public TimeSpan? Timeout { get; init; }
}
