using LibraryMS.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace LibraryMS.Infrastructure.Caching;

// Adapter Pattern: Wraps StackExchange.Redis into our ICacheService abstraction.
// Application layer never imports StackExchange.Redis directly.
public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _connection;
    private readonly ILogger<RedisCacheService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public RedisCacheService(IConnectionMultiplexer connection, ILogger<RedisCacheService> logger)
    {
        _connection = connection;
        _logger = logger;
        _db = connection.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            var json = value.ToString();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection failed while trying to get cache key {Key}. Falling back to database.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value, JsonOptions);
            await _db.StringSetAsync(key, serialized, expiry ?? TimeSpan.FromMinutes(30));
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection failed while trying to set cache key {Key}. Operation skipped.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _db.KeyDeleteAsync(key);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection failed while trying to remove cache key {Key}. Operation skipped.", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();
            if (keys.Length > 0)
                await _db.KeyDeleteAsync(keys);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection failed while trying to remove cache keys with prefix {Prefix}. Operation skipped.", prefix);
        }
    }
}
