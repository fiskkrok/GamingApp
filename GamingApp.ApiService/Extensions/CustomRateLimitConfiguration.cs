using AspNetCoreRateLimit;

using Microsoft.Extensions.Options;
using Serilog.Context;

namespace GamingApp.ApiService.Extensions;

public class CustomRateLimitConfiguration(
    IOptions<IpRateLimitOptions> ipOptions,
    IOptions<ClientRateLimitOptions> clientOptions,
    ILogger<CustomRateLimitConfiguration> logger)
    : RateLimitConfiguration(ipOptions, clientOptions)
{
    public override void RegisterResolvers()
    {
        ClientResolvers.Add(new CustomClientResolveContributor(logger));
        IpResolvers.Add(new CustomIpResolveContributor(logger));
    }
}

public class CustomClientResolveContributor(ILogger logger) : IClientResolveContributor
{
    public Task<string?> ResolveClientAsync(HttpContext httpContext)
    {
        var correlationId = httpContext.TraceIdentifier;
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            var clientId = httpContext.Request.Headers["X-ClientId"].FirstOrDefault();
            logger.LogInformation("Rate limit check for client: {ClientId}", clientId);
            return Task.FromResult(clientId);
        }
    }

}

public class CustomIpResolveContributor(ILogger logger) : IIpResolveContributor
{
    public string? ResolveIp(HttpContext httpContext)
    {
        var correlationId = httpContext.TraceIdentifier;
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            var ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault()
                     ?? httpContext.Connection.RemoteIpAddress?.ToString();

            logger.LogInformation("Rate limit check for IP: {IP}", ip);
            return ip;
        }
    }
}
