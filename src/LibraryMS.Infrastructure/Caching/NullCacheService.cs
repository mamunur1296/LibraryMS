using LibraryMS.Application.Contracts.Services;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Infrastructure.Caching;

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
