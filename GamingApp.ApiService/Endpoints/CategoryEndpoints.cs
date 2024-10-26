using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

using GamingApp.ApiService.Services.Interfaces;

namespace GamingApp.ApiService.Endpoints;

public class GetCategoriesEndpoint : EndpointWithoutRequest<List<Category>>
{
    public GetCategoriesEndpoint(AppDbContext dbContext, ICacheService cache)
    {
        DbContext = dbContext;
        Cache = cache;
    }

    private ICacheService Cache { get; }
    private AppDbContext DbContext { get; }

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
                const string cacheKey = "categories";
                var cachedCategories = await Cache.GetAsync<List<Category>>(cacheKey);

                if (cachedCategories != null)
                {
                    Logger.LogInformation("Retrieved {Count} categories from cache", cachedCategories.Count);
                    await SendOkAsync(cachedCategories, ct);
                    return;
                }

                var categoriesFromDb = await DbContext.Categories
                    .Select(c => new Category
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Icon = c.Icon,
                        GameCount = c.Games.Count
                    })
                    .ToListAsync(ct);
                var time = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                var serializedCategories = JsonSerializer.Serialize(categoriesFromDb);
                await Cache.SetAsync(cacheKey, serializedCategories, time.AbsoluteExpirationRelativeToNow);

                Logger.LogInformation("Retrieved {Count} categories from database", categoriesFromDb.Count);
                await SendOkAsync(categoriesFromDb, ct);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error occurred while fetching categories");
                await SendErrorsAsync(500, ct);
            }
        }
    }
}
