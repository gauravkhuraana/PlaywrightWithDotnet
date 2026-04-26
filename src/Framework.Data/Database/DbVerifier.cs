using System.Data;
using Dapper;
using Framework.Configuration.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Framework.Data.Database;

/// <summary>Provider-agnostic database verification helper.</summary>
public interface IDbVerifier
{
    Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null);

    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);

    Task<int> ExecuteAsync(string sql, object? param = null);
}

/// <summary>Default <see cref="IDbVerifier"/> for SQL Server and PostgreSQL via Dapper.</summary>
public sealed class DbVerifier : IDbVerifier
{
    private readonly FrameworkSettings _settings;
    private readonly ILogger<DbVerifier> _logger;

    public DbVerifier(FrameworkSettings settings, ILogger<DbVerifier> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    private IDbConnection CreateConnection()
    {
        var cs = _settings.Database.ConnectionString
            ?? throw new InvalidOperationException("Framework:Database:ConnectionString is not configured.");

        return _settings.Database.Provider.ToLowerInvariant() switch
        {
            "sqlserver" or "mssql" => new SqlConnection(cs),
            "postgres" or "postgresql" or "npgsql" => new NpgsqlConnection(cs),
            _ => throw new NotSupportedException($"Database provider '{_settings.Database.Provider}' is not supported."),
        };
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null)
    {
        _logger.LogDebug("ExecuteScalar: {Sql}", sql);
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<T>(sql, param).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        _logger.LogDebug("Query: {Sql}", sql);
        using var conn = CreateConnection();
        return await conn.QueryAsync<T>(sql, param).ConfigureAwait(false);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        _logger.LogDebug("Execute: {Sql}", sql);
        using var conn = CreateConnection();
        return await conn.ExecuteAsync(sql, param).ConfigureAwait(false);
    }
}
