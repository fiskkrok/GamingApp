namespace GamingApp.ApiService.Extensions;

public static class LoggingExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        // Add your logging middleware implementation here
        return builder;
    }
    public static IServiceCollection AddLoggingExtensions(this IServiceCollection services)
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
            // Add other logging providers as needed
        });

        return services;
    }
}

public class CorrelationIdLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new CorrelationIdLogger(new HttpContextAccessor());
    }

    public void Dispose()
    {
    }
}

public class CorrelationIdLogger : ILogger
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdLogger(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IDisposable? BeginScope<TState>(TState state)
    {

        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var context = _httpContextAccessor.HttpContext;
        var correlationId = context?.TraceIdentifier ?? "N/A"; // Use the actual correlation ID from the context
        var message = formatter(state, exception);
        Console.WriteLine($"[{correlationId}] {logLevel}: {message}");
    }
}

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            logger.LogInformation("Handling request: {Path}", context.Request.Path);
            await next(context);
            logger.LogInformation("Finished handling request.");
        }
    }
}
