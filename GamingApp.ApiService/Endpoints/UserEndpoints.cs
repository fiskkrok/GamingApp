using FastEndpoints;

using GamingApp.ApiService.Data;

using Microsoft.EntityFrameworkCore;

using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Extensions;
using GamingApp.ApiService.Services.Interfaces;

using Serilog.Context;

namespace GamingApp.ApiService.Endpoints;

public class GetUserStatsEndpoint : EndpointWithoutRequest<UserStats>
{
    public GetUserStatsEndpoint(AppDbContext dbContext, ICacheService cache)
    {
        DbContext = dbContext;
        Cache = cache;
    }

    private ICacheService Cache { get; }
    private AppDbContext DbContext { get; }
    public override void Configure()
    {
        Get("/userStats");

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
                await SendErrorsAsync(500,  ct);
            }
        }
    }
}

public class CheckUsernameUniquenessEndpoint : EndpointWithoutRequest<bool>
{
    public CheckUsernameUniquenessEndpoint(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    private AppDbContext DbContext { get; }
    public override void Configure()
    {
        Get("/userProfile/checkUsername");
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
                await SendErrorsAsync(500, ct);
            }
        }
    }
}

public class CreateUserProfileEndpoint : Endpoint<CreateUserProfileRequest, User>
{
    public CreateUserProfileEndpoint(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    private AppDbContext DbContext { get; }
    public override void Configure()
    {
        Post("/userProfile");

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

                var user = new User(
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
                await SendErrorsAsync(500, ct);
            }
        }
    }
}
    //    var validationResult = await _validator.ValidateAsync(parameter);
    //    if (!validationResult.IsValid)
    //    {
    //        return Results.BadRequest(
    //            validationResult.Errors.Select(e => new ApiError(e.ErrorMessage)));
    //    }

    //    return await next(context);
    //}

public class CreateUserProfileRequest
{
    public required string InGameUserName { get; set; }
}

