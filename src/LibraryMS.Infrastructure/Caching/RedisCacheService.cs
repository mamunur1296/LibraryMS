using LibraryMS.Application.Contracts.Services;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LibraryMS.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public string Configuration { get; init; } = "localhost:6379,abortConnect=false";
}

internal sealed class NullCacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, (byte[] Value, DateTime Expiry)> _cache = new();
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiry > DateTime.UtcNow)
            {
                var json = System.Text.Encoding.UTF8.GetString(entry.Value);
                return Task.FromResult(JsonSerializer.Deserialize<T>(json, JsonOptions));
            }
            _cache.TryRemove(key, out _);
        }
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        _cache[key] = (bytes, DateTime.UtcNow.Add(expiry ?? TimeSpan.FromMinutes(30)));
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var keys = _cache.Keys.Where(k => k.StartsWith(prefix)).ToArray();
        foreach (var key in keys) _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }
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
