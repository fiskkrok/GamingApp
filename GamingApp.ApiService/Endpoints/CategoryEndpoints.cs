using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Serilog.Context;

namespace GamingApp.ApiService.Endpoints;

public class GetCategoriesEndpoint : EndpointWithoutRequest<List<Category>>
{
    public override void Configure()
    {
        Get("/categories");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var cacheKey = "categories";
                var cachedCategories = await Cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedCategories))
                {
                    var categories = JsonSerializer.Deserialize<List<Category>>(cachedCategories);
                    Logger.LogInformation("Retrieved {Count} categories from cache", categories.Count);
                    await SendOkAsync(categories, ct);
                    return;
                }

                var categoriesFromDb = await DbContext.Categories
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Icon,
                        GameCount = c.Games.Count
                    })
                    .ToListAsync(ct);

                var serializedCategories = JsonSerializer.Serialize(categoriesFromDb);
                await Cache.SetStringAsync(cacheKey, serializedCategories, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                Logger.LogInformation("Retrieved {Count} categories from database", categoriesFromDb.Count);
                await SendOkAsync(categoriesFromDb, ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred while fetching categories");
                await SendErrorsAsync(500, "An error occurred while fetching categories", ct);
            }
        }
    }
}
