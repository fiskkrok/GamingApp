using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Extensions;

using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Endpoints;

public static class UserHelper
{
    public static async Task<User?> EnsureUserExistsAsync(
        HttpContext context,
        AppDbContext dbContext,
        ILogger logger,
        CancellationToken ct = default)
    {
        try
        {
            // Get the SID from the authenticated user
            var identityServerSid = context.GetRequiredIdentityServerSid();

            // Try to find existing user
            var user = await dbContext.Users
                .Include(u => u.GameSessions)
                .Include(u => u.AchievementsUnlocked)
                .Include(u => u.PlayedGames)
                .FirstOrDefaultAsync(u => u.IdentityServerSid == identityServerSid, ct);

            if (user != null)
            {
                logger.LogInformation("Found existing user profile for SID: {Sid}", identityServerSid);
                return user;
            }

            // If no user exists, we'll create one using claims from Identity Server
            var (name, email) = context.GetNameAndEmail(logger);

            user = new User(
                identityServerSid: identityServerSid,
                username: name,
                email: email,
                createdAt: DateTime.UtcNow,
                inGameUserName: "TempUsername_" + Guid.NewGuid().ToString("N")[..8] // Temporary username
            );

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(ct);

            logger.LogInformation("Created new user profile for SID: {Sid}", identityServerSid);
            return user;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            logger.LogError(ex, "Authentication error while ensuring user exists");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error ensuring user exists");
            throw;
        }
    }
}

// Extension method for UserProfile responses
public static class UserExtensions
{
    public static UserProfileResponse ToProfileResponse(this User user)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            InGameUserName = user.InGameUserName,
            Bio = user.Bio,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            FavoriteGame = user.FavoriteGame,
            //TotalPlayTime = user.TotalPlayTime,
            //GamesPlayed = user.PlayedGames.Count,
            //AchievementsUnlocked = user.AchievementsUnlocked.Count,
            //RecentAchievements = user.RecentAchievements
        };
    }
}

// Response model for user profile
public class UserProfileResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string InGameUserName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? FavoriteGame { get; set; }
    //public TimeSpan TotalPlayTime { get; set; }
    //public int GamesPlayed { get; set; }
    //public int AchievementsUnlocked { get; set; }
    //public List<Achievement> RecentAchievements { get; set; } = [];
    public bool IsComplete => !string.IsNullOrEmpty(InGameUserName) && InGameUserName.StartsWith("TempUsername_") == false;
}
