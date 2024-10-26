using GamingApp.ApiService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Data.SeedData;

public class UserSeedData
{
    public static async Task<User> GetSeedData(List< Game> games)
    {
        var user = new User(
            "mock_sid",
            "MockUser",
            "mockuser@example.com",
            DateTime.UtcNow,
            "MockGamer"
        );
        foreach (var game in games)
        {
            user.PlayedGames.Add(game);
        }
        return user;
    }
}
