using GamingApp.ApiService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Data.SeedData;

public class GameSessionSeedData
{
    private static readonly Random Random = new();

    public static async Task<List<GameSession>> GetSeedData(AppDbContext dbContext, User user)
    {
        var games = await dbContext.Games.Take(3).OrderBy(a => a.Genre).ToListAsync();

        var gameSessions = new List<GameSession>();

        foreach (var game in games)
        {
            var gameSession = new GameSession
            {
                UserId = user.Id,
                GameId = game.Id,
                StartTime = DateTime.UtcNow.AddDays(-Random.Next(1, 30)),
                EndTime = DateTime.UtcNow.AddDays(-Random.Next(1, 30)),
                Score = Random.Next(100, 1000),
                User = user,
                Game = game
            };

            gameSessions.Add(gameSession);
        }

        return gameSessions;
    }
}
