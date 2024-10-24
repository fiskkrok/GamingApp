using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Services;
using GamingApp.ApiService.Services.Interfaces;
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

    private static async ValueTask<IResult> GetAllGamesAsync(
        AppDbContext context,
        ILogger<Program> logger,
        ICacheService cache,
        [FromRoute] int max)
    {
        try
        {
            var cacheKey = CacheKeys.GetGamesKey(max);
            var cachedGames = await cache.GetAsync<List<Game>>(cacheKey);

            if (cachedGames != null)
            {
                logger.LogInformation("Retrieved {Count} games from cache", cachedGames.Count);
                return Results.Ok(cachedGames);
            }

            var gamesFromDb = await context.Games
                .Include(g => g.Genre)
                .Take(max)
                .ToListAsync();

            await cache.SetAsync(cacheKey, gamesFromDb);

            logger.LogInformation("Retrieved {Count} games from database", gamesFromDb.Count);
            return Results.Ok(gamesFromDb);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching games");
            return Results.Problem("An error occurred while fetching games");
        }
    }

    private static async ValueTask<IResult> GetRecentGamesAsync(
        AppDbContext context,
        ILogger<Program> logger,
        IDistributedCache cache,
        [FromRoute] int count = 10)
    {
        try
        {
            var cacheKey = $"recentGames_{count}";
            var cachedRecentGames = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedRecentGames))
            {
                var recentGames = JsonSerializer.Deserialize<List<Game>>(cachedRecentGames);
                logger.LogInformation("Retrieved {Count} recent games from cache", recentGames.Count);
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

            logger.LogInformation("Retrieved {Count} recent games from database", recentGamesFromDb.Count);
            return Results.Ok(recentGamesFromDb);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching recent games");
            return Results.Problem("An error occurred while fetching recent games");
        }
    }

    private static async ValueTask<IResult> GetRecommendedGamesAsync(
        AppDbContext context,
        ILogger<Program> logger,
        IDistributedCache cache,
        [FromRoute] int count = 10)
    {
        try
        {
            var cacheKey = $"recommendedGames_{count}";
            var cachedRecommendedGames = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedRecommendedGames))
            {
                var recommendedGames = JsonSerializer.Deserialize<List<object>>(cachedRecommendedGames);
                logger.LogInformation("Retrieved {Count} recommended games from cache", recommendedGames.Count);
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

            logger.LogInformation("Retrieved {Count} recommended games from database", recommendedGamesFromDb.Count);
            return Results.Ok(recommendedGamesFromDb);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching recommended games");
            return Results.Problem("An error occurred while fetching recommended games");
        }
    }
}

//// Example: When a game is updated
//public static async Task<IResult> UpdateGameAsync(
//    AppDbContext context,
//    ICacheService cache,
//    Game game)
//{
//    // Update game logic...
//    await cache.RemoveByPrefixAsync(CacheKeys.Games);
//    await cache.RemoveByPrefixAsync(CacheKeys.RecommendedGames);
//    // ...
//}