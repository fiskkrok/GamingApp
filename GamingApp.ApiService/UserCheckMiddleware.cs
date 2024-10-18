using GamingApp.ApiService.Data;
using GamingApp.ApiService.Data.Models;
using GamingApp.ApiService.Endpoints;


namespace GamingApp.ApiService;

public class UserCheckMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        try
        {
            var user = await UserHelper.EnsureUserExistsAsync(context, dbContext);
            if (user != null) context.Items["UserId"] = user.Id;
        }
        catch (InvalidOperationException ex)
        {
           await Console.Error.WriteLineAsync($"Error in UserCheckMiddleware: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(
                "An error occurred while processing your request. Please try again later.");
            return;
        }

        await next(context);
    }
}

public static class UserCheckMiddlewareExtensions
{
    public static IApplicationBuilder UseUserCheck(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserCheckMiddleware>();
    }
}