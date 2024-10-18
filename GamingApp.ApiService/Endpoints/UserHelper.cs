using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GamingApp.ApiService.Endpoints;

public static class UserHelper
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

    public static async Task<User?> EnsureUserExistsAsync(HttpContext context, AppDbContext dbContext)
    {
        var identityServerSid = context.GetRequiredIdentityServerSid();
        if (Cache.TryGetValue(identityServerSid, out User? cachedUser)) return cachedUser;

        var user = await dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdentityServerSid == identityServerSid);

        if (user == null)
        {
            user = new User(
                identityServerSid,
                context.User.Identity?.Name ?? "Unknown",
                context.User.FindFirst("email")?.Value ?? "Unknown",
                DateTime.UtcNow,
                "Unknown",
                null,
                null,
                null
            );
            dbContext.Users.Add(user);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await Console.Error.WriteLineAsync($"Error saving new user to the database: {ex.Message}");
                throw new InvalidOperationException("An error occurred while creating the user. Please try again.", ex);
            }
        }

        Cache.Set(identityServerSid, user, TimeSpan.FromMinutes(10));
        return user;
    }
}
