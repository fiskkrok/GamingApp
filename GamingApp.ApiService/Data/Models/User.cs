using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingApp.ApiService.Data.Models;

public class User(
    string identityServerSid,
    string username,
    string email,
    DateTime createdAt,
    string inGameUserName,
    string? favoriteGame,
    string? bio,
    string? status)
{
    public int Id { get; init; }

    [Required] public string IdentityServerSid { get; init; } = identityServerSid;

    [Required] public string Username { get; init; } = username;

    [Required] public string Email { get; init; } = email;

    public DateTime CreatedAt { get; init; } = createdAt;

    [Required] public string InGameUserName { get; init; } = inGameUserName;

    public string? FavoriteGame { get; init; } = favoriteGame;

    public string? Bio { get; init; } = bio;

    public string? Status { get; init; } = status;

    // Navigation properties
    public ICollection<GameSession> GameSessions { get; init; } = (List<GameSession>) [];
    public ICollection<Achievement> AchievementsUnlocked { get; init; } = (List<Achievement>) [];
    public ICollection<Game> PlayedGames { get; init; } = (List<Game>) [];

    [NotMapped]
    public TimeSpan TotalPlayTime => TimeSpan.FromTicks(GameSessions.Sum(gs => (gs.EndTime - gs.StartTime).Ticks));

    [NotMapped] public List<Achievement> RecentAchievements => GetTopAchievements();

    private List<Achievement> GetTopAchievements(int count = 3)
    {
        return AchievementsUnlocked
            .OrderByDescending(a => a.Score)
            .Take(count)
            .ToList();
    }
}