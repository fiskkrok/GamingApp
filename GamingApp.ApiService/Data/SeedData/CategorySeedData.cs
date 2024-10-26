using GamingApp.ApiService.Data.Models;

namespace GamingApp.ApiService.Data.SeedData;

public class CategorySeedData
{
    public static List<Category> GetSeedData()
    {
        return new List<Category>
        {
            new Category { Name = "Action", GameCount = 0, Icon = "TempString" },
            new Category { Name = "Adventure", GameCount = 0, Icon = "TempString" },
            new Category { Name = "Role-Playing", GameCount = 0, Icon = "TempString" },
            new Category { Name = "Survival", GameCount = 0, Icon = "TempString" },
            new Category { Name = "Strategy", GameCount = 0, Icon = "TempString" },
            new Category { Name = "Simulation", GameCount = 0, Icon = "TempString" }
        };
    }
}
