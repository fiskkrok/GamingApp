using GamingApp.Web.Models;

namespace GamingApp.Web.Clients;

public class CategoryApiClient(HttpClient httpClient)
{
    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        var response = await httpClient.GetFromJsonAsync<List<Category>>("/categories");
        return response ?? Enumerable.Empty<Category>();
    }
}