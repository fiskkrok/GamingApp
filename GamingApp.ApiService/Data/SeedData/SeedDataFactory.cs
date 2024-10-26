using GamingApp.ApiService.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GamingApp.ApiService.Data.SeedData;

public static class SeedDataFactory
{
    private static List<Category>? Categories { get; set; }
    private static User? User { get; set; }
    private static List<Game>? Game { get; set; }
    public static object CreateSeedData(Type type)
    {

        if (type == typeof(Category))
        {
            return Categories = CategorySeedData.GetSeedData();
        }

        if (type == typeof(Game) && Categories != null) return Game = GameSeedData.GetSeedData(Categories);

        if (type == typeof(User) && Game != null) return User = UserSeedData.GetSeedData(Game).Result;

        if (type == typeof(GameSession) && Game != null && User != null)
            return GameSessionSeedData.GetSeedData(Game, User);

        throw new ArgumentException("Invalid type for seed data creation");
    }
}
