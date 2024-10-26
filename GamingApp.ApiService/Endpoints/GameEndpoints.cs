using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Services;
using GamingApp.ApiService.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Serilog.Context;

namespace GamingApp.ApiService.Endpoints;

public class GetAllGamesEndpoint : Endpoint<GetAllGamesRequest, List<Game>>
{
    public override void Configure()
    {
        Get("/games/{max:int}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllGamesRequest req, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var cacheKey = CacheKeys.GetGamesKey(req.Max);
                var cachedGames = await Cache.GetAsync<List<Game>>(cacheKey);

                if (cachedGames != null)
                {
                    Logger.LogInformation("Retrieved {Count} games from cache", cachedGames.Count);
                    await SendOkAsync(cachedGames, ct);
                    return;
                }

                var gamesFromDb = await DbContext.Games
                    .Include(g => g.Genre)
                    .Take(req.Max)
                    .ToListAsync(ct);

                await Cache.SetAsync(cacheKey, gamesFromDb);

                Logger.LogInformation("Retrieved {Count} games from database", gamesFromDb.Count);
                await SendOkAsync(gamesFromDb, ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred while fetching games");
                await SendErrorsAsync(500, "An error occurred while fetching games", ct);
            }
        }
    }
}

public class GetRecentGamesEndpoint : Endpoint<GetRecentGamesRequest, List<Game>>
{
    public override void Configure()
    {
        Get("/recentGames/{count:int}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetRecentGamesRequest req, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var cacheKey = CacheKeys.GetRecentGamesKey(req.Count);
                var cachedRecentGames = await Cache.GetAsync<List<Game>>(cacheKey);

                if (cachedRecentGames != null)
                {
                    Logger.LogInformation("Retrieved {Count} recent games from cache", cachedRecentGames.Count);
                    await SendOkAsync(cachedRecentGames, ct);
                    return;
                }

                var recentGamesFromDb = await DbContext.Games
                    .Include(g => g.Genre)
                    .OrderByDescending(g => g.CreatedAt)
                    .Take(req.Count)
                    .ToListAsync(ct);

                await Cache.SetAsync(cacheKey, recentGamesFromDb);

                Logger.LogInformation("Retrieved {Count} recent games from database", recentGamesFromDb.Count);
                await SendOkAsync(recentGamesFromDb, ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred while fetching recent games");
                await SendErrorsAsync(500, "An error occurred while fetching recent games", ct);
            }
        }
    }
}

public class GetRecommendedGamesEndpoint : Endpoint<GetRecommendedGamesRequest, List<object>>
{
    public override void Configure()
    {
        Get("/recommendedGames/{count:int}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetRecommendedGamesRequest req, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var cacheKey = CacheKeys.GetRecommendedGamesKey(req.Count);
                var cachedRecommendedGames = await Cache.GetAsync<List<object>>(cacheKey);

                if (cachedRecommendedGames != null)
                {
                    Logger.LogInformation("Retrieved {Count} recommended games from cache", cachedRecommendedGames.Count);
                    await SendOkAsync(cachedRecommendedGames, ct);
                    return;
                }

                var recommendedGamesFromDb = await DbContext.Games
                    .Include(g => g.Genre)
                    .Include(g => g.GameSessions)
                    .OrderByDescending(g => g.GameSessions.Count)
                    .Take(req.Count)
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
                    .ToListAsync(ct);

                await Cache.SetAsync(cacheKey, recommendedGamesFromDb);

                Logger.LogInformation("Retrieved {Count} recommended games from database", recommendedGamesFromDb.Count);
                await SendOkAsync(recommendedGamesFromDb, ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred while fetching recommended games");
                await SendErrorsAsync(500, "An error occurred while fetching recommended games", ct);
            }
        }
    }
}

public class GetAllGamesRequest
{
    public int Max { get; set; }
}

public class GetRecentGamesRequest
{
    public int Count { get; set; }
}

public class GetRecommendedGamesRequest
{
    public int Count { get; set; }
}
