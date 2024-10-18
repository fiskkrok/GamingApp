using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamingApp.ApiService.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        app.MapGet("/games/{max:int}", GetAllGamesAsync).RequireAuthorization();
        app.MapGet("/recentGames/{count:int}", GetRecentGamesAsync).RequireAuthorization();
        app.MapGet("/recommendedGames/{count:int}", GetRecommendedGamesAsync).RequireAuthorization();
    }

    private static async ValueTask<IResult> GetAllGamesAsync(AppDbContext context, ILogger<Program> logger,[FromRoute]int max)
    {
        try
        {
            var games = await context.Games
                .Include(g => g.Genre).Take(max)
                .ToListAsync();

            logger.LogInformation("Retrieved {Count} games", games.Count);
            return Results.Ok(games);
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
        [FromRoute] int count = 10)
    {
        try
        {
            var recentGames = await context.Games
                .Include(g => g.Genre)
                .OrderByDescending(g => g.CreatedAt)
                .Take(count)
                .ToListAsync();

            logger.LogInformation("Retrieved {Count} recent games", recentGames.Count);
            return Results.Ok(recentGames);
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
        [FromRoute] int count = 10)
    {
        try
        {
            var recommendedGames = await context.Games
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

            logger.LogInformation("Retrieved {Count} recommended games", recommendedGames.Count);
            return Results.Ok(recommendedGames);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching recommended games");
            return Results.Problem("An error occurred while fetching recommended games");
        }
    }
}