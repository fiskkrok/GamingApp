using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;

namespace GamingApp.ApiService.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        app.MapGet("/categories", GetCategoriesAsync).RequireAuthorization();
    }

    private static async ValueTask<IResult> GetCategoriesAsync(
        AppDbContext context,
        ILogger<Program> logger)
    {
        try
        {
            var categories = await context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Icon,
                    GameCount = c.Games.Count
                })
                .ToListAsync();

            logger.LogInformation("Retrieved {Count} categories", categories.Count);
            return Results.Ok(categories);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching categories");
            return Results.Problem("An error occurred while fetching categories");
        }
    }
}