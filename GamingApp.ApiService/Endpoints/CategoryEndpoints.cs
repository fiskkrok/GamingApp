﻿using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace GamingApp.ApiService.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        app.MapGet("/categories", GetCategoriesAsync).RequireAuthorization();
    }

    private static async ValueTask<IResult> GetCategoriesAsync(
        AppDbContext context,
        ILogger<Program> logger,
        IDistributedCache cache,
        HttpContext httpContext)
    {
        var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        try
        {
            var cacheKey = "categories";
            var cachedCategories = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedCategories))
            {
                var categories = JsonSerializer.Deserialize<List<Category>>(cachedCategories);
                logger.LogInformation("Retrieved {Count} categories from cache. Correlation ID: {CorrelationId}", categories.Count, correlationId);
                return Results.Ok(categories);
            }

            var categoriesFromDb = await context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Icon,
                    GameCount = c.Games.Count
                })
                .ToListAsync();

            var serializedCategories = JsonSerializer.Serialize(categoriesFromDb);
            await cache.SetStringAsync(cacheKey, serializedCategories, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            logger.LogInformation("Retrieved {Count} categories from database. Correlation ID: {CorrelationId}", categoriesFromDb.Count, correlationId);
            return Results.Ok(categoriesFromDb);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching categories. Correlation ID: {CorrelationId}", correlationId);
            return Results.Problem("An error occurred while fetching categories");
        }
    }
}
