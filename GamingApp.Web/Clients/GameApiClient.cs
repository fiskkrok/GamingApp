using GamingApp.Web.Models;
using Grpc.Core;
using Microsoft.FluentUI.AspNetCore.Components;
using Polly;

namespace GamingApp.Web.Clients;

public class GameApiClient(HttpClient httpClient)
{
    public async Task<IEnumerable<Game>> GetGamesAsync(int maxItems = 10)
    {
        var response = await httpClient.GetFromJsonAsync<List<Game>>($"/games/{maxItems}");
        return response ?? Enumerable.Empty<Game>();
    }

    public async Task<IEnumerable<Game>> GetRecentGamesAsync(int count)
    {
        var response = await httpClient.GetFromJsonAsync<List<Game>>($"/recentGames/{count}");
        return response ?? Enumerable.Empty<Game>();
    }

    public async Task<IEnumerable<Game>> GetRecommendedGamesAsync(int count)
    {
        var response = await httpClient.GetFromJsonAsync<List<Game>>($"/recommendedGames/{count}");
        return response ?? Enumerable.Empty<Game>();
    }
}

public class UserStats
{
    public TimeSpan TotalPlayTime { get; set; }
    public int GamesPlayed { get; set; }
    public int AchievementsUnlocked { get; set; }
}