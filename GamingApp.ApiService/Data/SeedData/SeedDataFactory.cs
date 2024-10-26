using GamingApp.ApiService.Data.Models;

namespace GamingApp.ApiService.Data.SeedData;

public class SeedDataFactory
{
    public static object CreateSeedData(Type type)
    {
        if (type == typeof(Category))
        {
            return CategorySeedData.GetSeedData();
        }
        else if (type == typeof(Game))
        {
            return GameSeedData.GetSeedData(new List<Category>());
        }
        else if (type == typeof(User))
        {
            return UserSeedData.GetSeedData(new AppDbContext(new DbContextOptions<AppDbContext>()));
        }
        else if (type == typeof(GameSession))
        {
            return GameSessionSeedData.GetSeedData(new AppDbContext(new DbContextOptions<AppDbContext>()), new User());
        }
        else
        {
            throw new ArgumentException("Invalid type for seed data creation");
        }
    }
}
