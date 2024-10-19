using GamingApp.Web.Models;

namespace GamingApp.Web.Clients;

public class UserApiClient(HttpClient httpClient)
{
    public async Task<UserProfile?> GetUserProfileAsync()
    {
        return await GetFromApiAsync<UserProfile>("/userProfile");
    }

    public async Task<UserProfile> CreateUserProfileAsync(string inGameUserName)
    {
        var response = await httpClient.PostAsJsonAsync("/userProfile", new { InGameUserName = inGameUserName });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserProfile>() ??
               throw new InvalidOperationException("Failed to create user profile");
    }

    public async Task<bool> IsInGameUserNameUniqueAsync(string inGameUserName)
    {
        return await GetFromApiAsync<bool>($"/userProfile/checkUsername?username={Uri.EscapeDataString(inGameUserName)}");
    }

    public async Task UpdateUserProfileAsync(UserProfile userProfile)
    {
        var response = await httpClient.PutAsJsonAsync("/userProfile", userProfile);
        response.EnsureSuccessStatusCode();
    }

    private async Task<T?> GetFromApiAsync<T>(string endpoint)
    {
        var response = await httpClient.GetFromJsonAsync<T>(endpoint);
        return response;
    }
}
