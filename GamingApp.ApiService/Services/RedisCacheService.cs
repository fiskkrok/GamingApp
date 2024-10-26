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
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters =
            {
                new JsonStringEnumConverter(),
                new TimeSpanConverter()
            }
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
                if (string.IsNullOrEmpty(cached)) return default;

                try
                {
                    return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing cached item {Key}", key);
                    await RemoveAsync(key);
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached item {Key}", key);
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
                _logger.LogError(ex, "Error caching item {Key}", key);
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
                _logger.LogError(ex, "Error removing cached item {Key}", key);
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
public class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeSpan.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
