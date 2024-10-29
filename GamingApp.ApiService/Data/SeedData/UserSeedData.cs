using GamingApp.ApiService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Data.SeedData;

public class UserSeedData
{
    public static List<User> GetSeedData(List< Game> games)
    {
            return
            [
                new User(
                    "mock_sid",
                    "MockUser",
                    "mockuser@example.com",
                    DateTime.UtcNow,
                    "MockGamer"
                ){
                    PlayedGames = [..games.Take(3)]
                },
                new User(
                    "DC9BED62B3766F20841BBB5231639F8E",
                    "Bob Smith",
                    "BobSmith@email.com",
                    DateTime.UtcNow,
                    "BobTheGamer"
                ){
                    PlayedGames = [..games.Take(3)]
                },

            ];
    }
}
