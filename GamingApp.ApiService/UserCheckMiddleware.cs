using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Extensions;

using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService;

public class UserCheckMiddleware(RequestDelegate next, ILogger<UserCheckMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        var correlationId = context.TraceIdentifier;
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                // Only check for authenticated users
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var identityServerSid = context.GetRequiredIdentityServerSid();
                    logger.LogInformation("Checking user with SID: {Sid}", identityServerSid);

                    var user = await dbContext.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.IdentityServerSid == identityServerSid);

                    if (user != null)
                    {
                        context.Items["UserId"] = user.Id;
                        logger.LogInformation("Found existing user profile for SID: {Sid}", identityServerSid);
                    }
                    else
                    {
                        // If this is a profile-related endpoint, let it handle the missing profile
                        if (context.Request.Path.StartsWithSegments("/userProfile"))
                        {
                            logger.LogInformation("No profile found for SID: {Sid} - Allowing profile creation", identityServerSid);
                        }
                        else
                        {
                            logger.LogInformation("No profile found for SID: {Sid} - Redirecting to profile creation", identityServerSid);
                            context.Response.StatusCode = StatusCodes.Status307TemporaryRedirect;
                            context.Response.Headers.Location = "/profile";
                            return;
                        }
                    }
                }

                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in UserCheckMiddleware");
                throw;
            }
        }
    }
}

public static class UserCheckMiddlewareExtensions
{
    public static IApplicationBuilder UseUserCheck(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserCheckMiddleware>();
    }
}
