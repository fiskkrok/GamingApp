namespace GamingApp.ApiService.Data.Models;

public class UserStats
{
    public TimeSpan TotalPlayTime { get; set; }
    public int GamesPlayed { get; set; }
    public int AchievementsUnlocked { get; set; }
}

