using GamingApp.ApiService.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;
using System.Text.Json;
using Serilog.Context;

namespace GamingApp.ApiService.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private const int DefaultCacheMinutes = 5;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            try
            {
                var cached = await _cache.GetStringAsync(key);
                return string.IsNullOrEmpty(cached) ? default : JsonSerializer.Deserialize<T>(cached, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached item {Key} with CorrelationId {CorrelationId}", key, correlationId);
                return default;
            }
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(DefaultCacheMinutes)
                };

                var serialized = JsonSerializer.Serialize(value, _jsonOptions);
                await _cache.SetStringAsync(key, serialized, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching item {Key} with CorrelationId {CorrelationId}", key, correlationId);
            }
        }
    }

    public async Task RemoveAsync(string key)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached item {Key} with CorrelationId {CorrelationId}", key, correlationId);
            }
        }
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            // Implementation depends on your Redis setup
            // This is a placeholder for pattern-based cache invalidation
            _logger.LogWarning("RemoveByPrefixAsync not implemented with CorrelationId {CorrelationId}", correlationId);
            await Task.CompletedTask;
        }
    }
}
