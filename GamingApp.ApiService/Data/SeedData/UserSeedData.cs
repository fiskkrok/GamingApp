using GamingApp.ApiService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Data.SeedData;

public class UserSeedData
{
    public static async Task<User> GetSeedData(AppDbContext dbContext)
    {
        var user = new User(
            "mock_sid",
            "MockUser",
            "mockuser@example.com",
            DateTime.UtcNow,
            "MockGamer"
        );

        var games = await dbContext.Games.Take(3).ToListAsync();
        foreach (var game in games)
        {
            user.PlayedGames.Add(game);
        }

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }
}
