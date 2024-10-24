namespace GamingApp.ApiService.Services;

public static class CacheKeys
{
    public const string Games = "games";
    public const string Categories = "categories";
    public const string RecentGames = "recent_games";
    public const string RecommendedGames = "recommended_games";

    public static string GetGamesKey(int max) => $"{Games}_{max}";
    public static string GetRecentGamesKey(int count) => $"{RecentGames}_{count}";
    public static string GetRecommendedGamesKey(int count) => $"{RecommendedGames}_{count}";
}