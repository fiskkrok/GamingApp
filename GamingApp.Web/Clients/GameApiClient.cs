using GamingApp.Web.Models;

namespace GamingApp.Web.Clients;

public class GameApiClient(HttpClient httpClient)
{
    public async Task<IEnumerable<Game>> GetGamesAsync(int maxItems = 20)
    {
        return await GetFromApiAsync<List<Game>>($"/games/{maxItems}") ?? Enumerable.Empty<Game>();
    }

    public async Task<IEnumerable<Game>> GetRecentGamesAsync(int count)
    {
        return await GetFromApiAsync<List<Game>>($"/recentGames/{count}") ?? Enumerable.Empty<Game>();
    }

    public async Task<IEnumerable<Game>> GetRecommendedGamesAsync(int count)
    {
        return await GetFromApiAsync<List<Game>>($"/recommendedGames/{count}") ?? Enumerable.Empty<Game>();
    }

    private async Task<T?> GetFromApiAsync<T>(string endpoint)
    {
        var response = await httpClient.GetFromJsonAsync<T>(endpoint);
        return response;
    }
}
