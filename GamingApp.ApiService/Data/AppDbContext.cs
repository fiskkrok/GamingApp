using Polly;
using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data.Models;

namespace GamingApp.ApiService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; } // Add DbSet for Game
    public DbSet<GameSession> GameSessions { get; set; } // Add DbSet for GameSession
    public DbSet<User> Users { get; set; } // Add DbSet for User
    public DbSet<Category> Categories { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    private static readonly Random Random = new();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public static async Task EnsureDbCreatedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3));

        await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                await InitializeDataAsync(dbContext);
            }
            catch (Exception ex)
            {
                // Log the error (you can replace this with your logging mechanism)
                await Console.Error.WriteLineAsync($"An error occurred while ensuring the database is created: {ex.Message}");
                throw;
            }
        });
    }

    private static async Task InitializeDataAsync(AppDbContext dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var newCategories = new[]
                {
                    new Category { Name = "Action", GameCount = 0, Icon = "TempString" },
                    new Category { Name = "Adventure", GameCount = 0, Icon = "TempString" },
                    new Category { Name = "Role-Playing", GameCount = 0, Icon = "TempString" },
                    new Category { Name = "Survival", GameCount = 0, Icon = "TempString" },
                    new Category { Name = "Strategy", GameCount = 0, Icon = "TempString" },
                    new Category { Name = "Simulation", GameCount = 0, Icon = "TempString" }
                };
                await dbContext.Categories.AddRangeAsync(newCategories);
                await dbContext.SaveChangesAsync();

                await dbContext.Games.AddRangeAsync((await GenerateMockGames(dbContext)));
                await dbContext.SaveChangesAsync();

                var user = await GenerateMockUser(dbContext);

                await GenerateMockGameSessions(dbContext, user);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    private static async Task<List<Game>> GenerateMockGames(AppDbContext dbContext)
    {
        var categories = await dbContext.Categories
            .Select(c => new
            {
                c.Id,
                c.Name
            })
            .ToListAsync();
        return [
            new Game
            {
                Name = "Space Adventure",
                Description = "A thrilling journey through the galaxy.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(a => a.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Mystery Mansion",
                Description = "Solve puzzles and escape the haunted mansion.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(a => a.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Zombie Apocalypse",
                Description = "Survive the undead in a post-apocalyptic world.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Fantasy Quest",
                Description = "Embark on an epic quest in a magical realm.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Role-Playing")).Id
            },

            new Game
            {
                Name = "Racing Rivals",
                Description = "Compete against the best racers in high-speed challenges.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Alien Invasion",
                Description = "Defend Earth from an alien attack.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Dungeon Crawler",
                Description = "Explore dangerous dungeons filled with monsters.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Role-Playing")).Id
            },

            new Game
            {
                Name = "Stealth Assassin",
                Description = "Use stealth to eliminate your enemies in this tactical game.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Galactic Trader",
                Description = "Build your trading empire across the stars.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Simulation")).Id
            },

            new Game
            {
                Name = "Pirate Adventure",
                Description = "Sail the seas in search of treasure and adventure.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Adventure")).Id
            },

            new Game
            {
                Name = "Cyberpunk City",
                Description = "Navigate a futuristic city full of crime and mystery.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Medieval Siege",
                Description = "Build your army and lay siege to enemy castles.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Strategy")).Id
            },

            new Game
            {
                Name = "Monster Hunter",
                Description = "Track and hunt down mythical creatures.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Super Ninja",
                Description = "Master martial arts and defeat your enemies.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "City Builder",
                Description = "Design and manage your own city.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Simulation")).Id
            },

            new Game
            {
                Name = "Wild West Showdown",
                Description = "Experience the wild west in this fast-paced action game.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture1.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            },

            new Game
            {
                Name = "Underwater Exploration",
                Description = "Explore the deep sea and discover hidden treasures.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture2.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Adventure")).Id
            },
            new Game
            {
                Name = "Time Traveler",
                Description = "Travel through time to solve mysteries and change history.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture3.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Adventure")).Id
            },
            new Game
            {
                Name = "Survival Island",
                Description = "Survive on a deserted island by gathering resources and building shelter.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture4.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Survival")).Id
            },
            new Game
            {
                Name = "Air Combat",
                Description = "Engage in thrilling aerial dogfights.",
                PictureUrl = "https://gameapp.blob.core.windows.net/lalala/picture5.webp",
                CreatedAt = DateTime.UtcNow,
                GenreId = categories.First(o => o.Name.Equals("Action")).Id
            }
        ];
    }

    private static async Task<User> GenerateMockUser(AppDbContext dbContext)
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

    private static async Task GenerateMockGameSessions(AppDbContext dbContext, User user)
    {
        var games = await dbContext.Games.Take(3).OrderBy(a => a.Genre).ToListAsync();

        foreach (var gameSession in games.Select(game => new GameSession
        {
            UserId = user.Id,
            GameId = game.Id,
            StartTime = DateTime.UtcNow.AddDays(-Random.Next(1,
                         30)),
            EndTime = DateTime.UtcNow.AddDays(-Random.Next(1,
                         30)),
            Score = Random.Next(100,
                         1000),
            User = user,
            Game = games.FirstOrDefault()
        }))
        {
            dbContext.GameSessions.Add(gameSession);
        }

        await dbContext.SaveChangesAsync();
    }
}
