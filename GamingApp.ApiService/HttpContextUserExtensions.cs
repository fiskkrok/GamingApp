using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GamingApp.ApiService;

public static class HttpContextUserExtensions
{
    public static string GetRequiredIdentityServerSid(this HttpContext httpContext)
    {
        if (httpContext.User.Identity is { IsAuthenticated: true } and ClaimsIdentity claimsIdentity)
        {
            var sidClaim = claimsIdentity.FindFirst("sid");
            if (sidClaim != null && !string.IsNullOrEmpty(sidClaim.Value)) return sidClaim.Value;
        }

        throw new InvalidOperationException("User is not authenticated or missing 'sid' claim");
    }

    public static int GetUserId(this HttpContext httpContext)
    {
        if (httpContext.Items["UserId"] is int userId) return userId;

        throw new InvalidOperationException(
            "User ID not found in context. Ensure UserCheckMiddleware is properly configured.");
    }

    public static (string name, string email) GetNameAndEmail(this HttpContext httpContext, ILogger logger)
    {
        var user = httpContext.User;

        // Log all available claims for debugging

        logger.LogInformation("Available claims:");
        foreach (var claim in user.Claims) logger.LogInformation("Claim Type: {ClaimType}, Value: {ClaimValue}", claim.Type, claim.Value);

        var name = user.FindFirst(ClaimTypes.Name)?.Value
                   ?? user.FindFirst("name")?.Value
                   ?? user.FindFirst("sub")?.Value // Try 'sub' claim as a fallback
                   ?? "Unknown";
        var email = user.FindFirst(ClaimTypes.Email)?.Value
                    ?? user.FindFirst("email")?.Value
                    ?? "Unknown";

        logger.LogInformation("Extracted Name: {Name}, Email: {Email}",name, email);

        if (name == "Unknown" || email == "Unknown")
            throw new InvalidOperationException("Failed to extract name and email from user claims.");

        return (name, email);
    }
}