using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace GamingApp.ApiService.Endpoints;

public class GetUserStatsEndpoint : EndpointWithoutRequest<UserStats>
{
    public override void Configure()
    {
        Get("/userStats");
        RequireAuthorization();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var userId = HttpContext.GetUserId();
                var user = await DbContext.Users
                    .Include(u => u.GameSessions)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (user == null)
                {
                    Logger.LogWarning("User with ID {UserId} not found", userId);
                    await SendNotFoundAsync(ct);
                    return;
                }

                var userStats = new UserStats
                {
                    TotalPlayTime = TimeSpan.FromTicks(user.GameSessions.Sum(gs => (gs.EndTime - gs.StartTime).Ticks)),
                    GamesPlayed = user.GameSessions.Select(gs => gs.GameId).Distinct().Count(),
                    AchievementsUnlocked = user.AchievementsUnlocked?.Count ?? 0
                };

                Logger.LogInformation("Retrieved stats for user {UserId}", userId);
                await SendOkAsync(userStats, ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred while fetching user stats");
                await SendErrorsAsync(500, "An error occurred while fetching user stats", ct);
            }
        }
    }
}

public class CheckUsernameUniquenessEndpoint : EndpointWithoutRequest<bool>
{
    public override void Configure()
    {
        Get("/userProfile/checkUsername");
        RequireAuthorization();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            try
            {
                var username = HttpContext.Request.Query["username"].ToString();
                var exists = await DbContext.Users.AnyAsync(u => u.InGameUserName == username, ct);
                await SendOkAsync(!exists, ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred while checking username uniqueness");
                await SendErrorsAsync(500, "An error occurred while checking username uniqueness", ct);
            }
        }
    }
}

public class CreateUserProfileEndpoint : Endpoint<CreateUserProfileRequest, User>
{
    public override void Configure()
    {
        Post("/userProfile");
        RequireAuthorization();
    }

    public override async Task HandleAsync(CreateUserProfileRequest req, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            var identityServerSid = HttpContext.User.FindFirst("sid")?.Value;
            if (string.IsNullOrEmpty(identityServerSid))
            {
                await SendUnauthorizedAsync(ct);
                return;
            }

            try
            {
                var (name, email) = HttpContext.GetNameAndEmail(Logger);

                var user = new User
                (
                    identityServerSid,
                    name,
                    email,
                    DateTime.UtcNow,
                    req.InGameUserName
                );

                DbContext.Users.Add(user);
                await DbContext.SaveChangesAsync(ct);

                await SendOkAsync(user, ct);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating user profile");
                await SendErrorsAsync(500, "An error occurred while creating the user profile.", ct);
            }
        }
    }
}

public class CreateUserProfileRequest
{
    public required string InGameUserName { get; set; }
}

public class UserStats
{
    public TimeSpan TotalPlayTime { get; set; }
    public int GamesPlayed { get; set; }
    public int AchievementsUnlocked { get; set; }
}
