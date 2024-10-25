using AspNetCoreRateLimit;

using Microsoft.Extensions.Options;

namespace GamingApp.ApiService.Extensions;

public class CustomRateLimitConfiguration : RateLimitConfiguration
{
    private readonly ILogger<CustomRateLimitConfiguration> _logger;

    public CustomRateLimitConfiguration(
        IOptions<IpRateLimitOptions> ipOptions,
        IOptions<ClientRateLimitOptions> clientOptions,
        ILogger<CustomRateLimitConfiguration> logger) : base(ipOptions, clientOptions)
    {
        _logger = logger;
    }

    public override void RegisterResolvers()
    {
        ClientResolvers.Add(new CustomClientResolveContributor(_logger));
        IpResolvers.Add(new CustomIpResolveContributor(_logger));
    }
}

public class CustomClientResolveContributor : IClientResolveContributor
{
    private readonly ILogger _logger;

    public CustomClientResolveContributor(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<string> ResolveClientAsync(HttpContext httpContext)
    {
        var correlationId = httpContext.TraceIdentifier;
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            var clientId = httpContext.Request.Headers["X-ClientId"].FirstOrDefault();
            _logger.LogInformation("Rate limit check for client: {ClientId}", clientId);
            return clientId;
        }
    }

}

public class CustomIpResolveContributor : IIpResolveContributor
{
    private readonly ILogger _logger;

    public CustomIpResolveContributor(ILogger logger)
    {
        _logger = logger;
    }

    public string? ResolveIp(HttpContext httpContext)
    {
        var correlationId = httpContext.TraceIdentifier;
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            var ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault()
                     ?? httpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation("Rate limit check for IP: {IP}", ip);
            return ip;
        }
    }
}
