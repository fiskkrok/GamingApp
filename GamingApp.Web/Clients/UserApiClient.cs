using GamingApp.Web.Models;

namespace GamingApp.Web.Clients;

public class UserApiClient(HttpClient httpClient)
{
    public async Task<UserProfile?> GetUserProfileAsync()
    {
        var response = await httpClient.GetAsync("/userProfile");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserProfile>();
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
        var response =
            await httpClient.GetAsync($"/userProfile/checkUsername?username={Uri.EscapeDataString(inGameUserName)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task UpdateUserProfileAsync(UserProfile userProfile)
    {
        var response = await httpClient.PutAsJsonAsync("/userProfile", userProfile);
        response.EnsureSuccessStatusCode();
    }
}