using Polly;
using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Data.SeedData;

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

        await using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3));

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
                    await Console.Error.WriteLineAsync(
                        $"An error occurred while ensuring the database is created: {ex.Message}");
                    throw;
                }
            });
        }
    }

    private static async Task InitializeDataAsync(AppDbContext dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var newCategories = SeedDataFactory.CreateSeedData(typeof(Category)) as List<Category>;
                await dbContext.Categories.AddRangeAsync(newCategories);
                await dbContext.SaveChangesAsync();

                var newGames = SeedDataFactory.CreateSeedData(typeof(Game)) as List<Game>;
                await dbContext.Games.AddRangeAsync(newGames);
                await dbContext.SaveChangesAsync();

                var user =  SeedDataFactory.CreateSeedData(typeof(User)) as User;
                await dbContext.AddRangeAsync(user);
                await dbContext.SaveChangesAsync();


                var newGameSessions =  SeedDataFactory.CreateSeedData(typeof(GameSession)) as List<GameSession>;
                await dbContext.GameSessions.AddRangeAsync(newGameSessions);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
