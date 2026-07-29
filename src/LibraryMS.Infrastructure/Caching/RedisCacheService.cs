using LibraryMS.Application.Contracts.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace LibraryMS.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public string Configuration { get; init; } = "localhost:6379";
}

/// <summary>
/// Adapter Pattern: Wraps StackExchange.Redis into our ICacheService abstraction.
/// Application layer never imports StackExchange.Redis directly.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _connection;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public RedisCacheService(IConnectionMultiplexer connection)
    {
        _connection = connection;
        _db = connection.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        var json = value.ToString();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var serialized = JsonSerializer.Serialize(value, JsonOptions);
        await _db.StringSetAsync(key, serialized, expiry ?? TimeSpan.FromMinutes(30));
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => await _db.KeyDeleteAsync(key);

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}*").ToArray();
        if (keys.Length > 0)
            await _db.KeyDeleteAsync(keys);
    }
}
