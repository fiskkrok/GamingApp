using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingApp.ApiService.Data.Models;

public class User
{
    public int Id { get; init; }

    [Required] public string IdentityServerSid { get; init; }

    [Required] public string Username { get; init; }

    [Required] public string Email { get; init; }

    public DateTime CreatedAt { get; init; }

    [Required] public string InGameUserName { get; init; }

    public string? FavoriteGame { get; init; }

    public string? Bio { get; init; }

    public string? Status { get; init; }

    // Navigation properties
    public ICollection<GameSession> GameSessions { get; init; } = new List<GameSession>();
    public ICollection<Achievement> AchievementsUnlocked { get; init; } = new List<Achievement>();
    public ICollection<Game> PlayedGames { get; init; } = new List<Game>();

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

    public User(string identityServerSid, string username, string email, DateTime createdAt, string inGameUserName)
    {
        IdentityServerSid = identityServerSid;
        Username = username;
        Email = email;
        CreatedAt = createdAt;
        InGameUserName = inGameUserName;
    }
}
