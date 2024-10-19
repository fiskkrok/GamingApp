using GamingApp.Web.Models;

namespace GamingApp.Web.Clients;

public class CategoryApiClient(HttpClient httpClient)
{
    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await GetFromApiAsync<List<Category>>("/categories") ?? Enumerable.Empty<Category>();
    }

    private async Task<T?> GetFromApiAsync<T>(string endpoint)
    {
        var response = await httpClient.GetFromJsonAsync<T>(endpoint);
        return response;
    }
}
