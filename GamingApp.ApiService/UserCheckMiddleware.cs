using GamingApp.ApiService.Data;
using GamingApp.ApiService.Endpoints;

namespace GamingApp.ApiService;

public class UserCheckMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserCheckMiddleware> _logger;

    public UserCheckMiddleware(RequestDelegate next, ILogger<UserCheckMiddleware> logger) : this(next)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            try
            {
                var user = await UserHelper.EnsureUserExistsAsync(context, dbContext);
                if (user != null) context.Items["UserId"] = user.Id;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error in UserCheckMiddleware with correlation ID: {CorrelationId}", correlationId);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(
                    "An error occurred while processing your request. Please try again later.");
                return;
            }

            await _next(context);
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
