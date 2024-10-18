namespace GamingApp.Web.Models;

public class UserProfile
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string InGameUserName { get; set; }
    public string? Bio { get; set; }
    public string? FavoriteGame { get; set; }
    public string? Status { get; set; }
    private IReadOnlyCollection<GameSession> GameSessions { get; set; } =  [];
    public IReadOnlyCollection<Achievement> AchievementsUnlocked { get; set; } = [];
    public ICollection<Game> GamesPlayed { get; set; } = (List<Game>) [];

    public TimeSpan TotalPlayTime => TimeSpan.FromTicks(GameSessions.Sum(gs => (gs.EndTime - gs.StartTime).Ticks));

    public List<Achievement> RecentAchievements()
    {
        return AchievementsUnlocked.OrderByDescending(a => a.UnlockedDate).Take(3).ToList();
    }
}