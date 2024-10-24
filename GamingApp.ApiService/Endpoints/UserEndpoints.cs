using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GamingApp.ApiService.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/userStats", GetUserStatsAsync).RequireAuthorization();
        app.MapGet("/userProfile/checkUsername", CheckUsernameUniqueness).RequireAuthorization();
        app.MapGet("/userProfile", CreateUserProfile).RequireAuthorization();
    }

    private static async ValueTask<IResult> GetUserStatsAsync(
        AppDbContext context,
        ILogger<Program> logger,
        HttpContext httpContext)
    {
        try
        {
            var userId = httpContext.GetUserId();
            var user = await context.Users
                .Include(u => u.GameSessions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found", userId);
                return Results.NotFound($"User with ID {userId} not found");
            }

            var userStats = new UserStats
            {
                TotalPlayTime = TimeSpan.FromTicks(user.GameSessions.Sum(gs => (gs.EndTime - gs.StartTime).Ticks)),
                GamesPlayed = user.GameSessions.Select(gs => gs.GameId).Distinct().Count(),
                AchievementsUnlocked = user.AchievementsUnlocked?.Count ?? 0
            };

            logger.LogInformation("Retrieved stats for user {UserId}", userId);
            return Results.Ok(userStats);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching user stats");
            return Results.Problem("An error occurred while fetching user stats");
        }
    }

    private static async Task<IResult> CheckUsernameUniqueness(AppDbContext context, [FromQuery] string username)
    {
        var exists = await context.Users.AnyAsync(u => u.InGameUserName == username);
        return Results.Ok(!exists);
    }

    private static async Task<IResult> CreateUserProfile(
        AppDbContext context,
        HttpContext httpContext,
        [FromBody] CreateUserProfileRequest? request,
        ILogger<Program> logger)
    {
        var identityServerSid = httpContext.User.FindFirst("sid")?.Value;
        if (string.IsNullOrEmpty(identityServerSid)) return Results.Unauthorized();

        try
        {
            var (name, email) = httpContext.GetNameAndEmail(logger);

            var user = new User
            (
                identityServerSid,
                name,
                email,
                DateTime.UtcNow,
                request?.InGameUserName ?? "Unknown"
            );

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Results.Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user profile");
            return Results.Problem("An error occurred while creating the user profile.");
        }
    }

    public class CreateUserProfileRequest
    {
        public required string InGameUserName { get; set; }
    }
}

public class UserStats
{
    public TimeSpan TotalPlayTime { get; set; }
    public int GamesPlayed { get; set; }
    public int AchievementsUnlocked { get; set; }
}
