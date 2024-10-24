using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace GamingApp.ApiService.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        app.MapGet("/games/{max:int}", GetAllGamesAsync).RequireAuthorization();
        app.MapGet("/recentGames/{count:int}", GetRecentGamesAsync).RequireAuthorization();
        app.MapGet("/recommendedGames/{count:int}", GetRecommendedGamesAsync).RequireAuthorization();
    }

    private static async ValueTask<IResult> GetAllGamesAsync(AppDbContext context, ILogger<Program> logger, IDistributedCache cache, [FromRoute]int max, HttpContext httpContext)
    {
        var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        try
        {
            var cacheKey = $"games_{max}";
            var cachedGames = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedGames))
            {
                var games = JsonSerializer.Deserialize<List<Game>>(cachedGames);
                logger.LogInformation("Retrieved {Count} games from cache. Correlation ID: {CorrelationId}", games.Count, correlationId);
                return Results.Ok(games);
            }

            var gamesFromDb = await context.Games
                .Include(g => g.Genre).Take(max)
                .ToListAsync();

            var serializedGames = JsonSerializer.Serialize(gamesFromDb);
            await cache.SetStringAsync(cacheKey, serializedGames, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            logger.LogInformation("Retrieved {Count} games from database. Correlation ID: {CorrelationId}", gamesFromDb.Count, correlationId);
            return Results.Ok(gamesFromDb);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching games. Correlation ID: {CorrelationId}", correlationId);
            return Results.Problem("An error occurred while fetching games");
        }
    }

    private static async ValueTask<IResult> GetRecentGamesAsync(
        AppDbContext context,
        ILogger<Program> logger,
        IDistributedCache cache,
        [FromRoute] int count = 10,
        HttpContext httpContext)
    {
        var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        try
        {
            var cacheKey = $"recentGames_{count}";
            var cachedRecentGames = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedRecentGames))
            {
                var recentGames = JsonSerializer.Deserialize<List<Game>>(cachedRecentGames);
                logger.LogInformation("Retrieved {Count} recent games from cache. Correlation ID: {CorrelationId}", recentGames.Count, correlationId);
                return Results.Ok(recentGames);
            }

            var recentGamesFromDb = await context.Games
                .Include(g => g.Genre)
                .OrderByDescending(g => g.CreatedAt)
                .Take(count)
                .ToListAsync();

            var serializedRecentGames = JsonSerializer.Serialize(recentGamesFromDb);
            await cache.SetStringAsync(cacheKey, serializedRecentGames, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            logger.LogInformation("Retrieved {Count} recent games from database. Correlation ID: {CorrelationId}", recentGamesFromDb.Count, correlationId);
            return Results.Ok(recentGamesFromDb);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching recent games. Correlation ID: {CorrelationId}", correlationId);
            return Results.Problem("An error occurred while fetching recent games");
        }
    }

    private static async ValueTask<IResult> GetRecommendedGamesAsync(
        AppDbContext context,
        ILogger<Program> logger,
        IDistributedCache cache,
        [FromRoute] int count = 10,
        HttpContext httpContext)
    {
        var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        try
        {
            var cacheKey = $"recommendedGames_{count}";
            var cachedRecommendedGames = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedRecommendedGames))
            {
                var recommendedGames = JsonSerializer.Deserialize<List<object>>(cachedRecommendedGames);
                logger.LogInformation("Retrieved {Count} recommended games from cache. Correlation ID: {CorrelationId}", recommendedGames.Count, correlationId);
                return Results.Ok(recommendedGames);
            }

            var recommendedGamesFromDb = await context.Games
                .Include(g => g.Genre)
                .Include(g => g.GameSessions)
                .OrderByDescending(g => g.GameSessions.Count)
                .Take(count)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Description,
                    g.PictureUrl,
                    g.CreatedAt,
                    Genre = g.Genre!.Name,
                    g.Developer,
                    Popularity = g.GameSessions.Count
                })
                .ToListAsync();

            var serializedRecommendedGames = JsonSerializer.Serialize(recommendedGamesFromDb);
            await cache.SetStringAsync(cacheKey, serializedRecommendedGames, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            logger.LogInformation("Retrieved {Count} recommended games from database. Correlation ID: {CorrelationId}", recommendedGamesFromDb.Count, correlationId);
            return Results.Ok(recommendedGamesFromDb);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching recommended games. Correlation ID: {CorrelationId}", correlationId);
            return Results.Problem("An error occurred while fetching recommended games");
        }
    }
}
