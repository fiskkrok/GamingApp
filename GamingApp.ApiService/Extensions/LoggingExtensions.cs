using Microsoft.Extensions.Logging;

namespace GamingApp.ApiService.Extensions;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddCorrelationIdLogging(this ILoggingBuilder builder)
    {
        builder.AddProvider(new CorrelationIdLoggerProvider());
        return builder;
    }

    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}

public class CorrelationIdLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new CorrelationIdLogger();
    }

    public void Dispose()
    {
    }
}

public class CorrelationIdLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var correlationId = Guid.NewGuid().ToString(); // This should be replaced with actual correlation ID logic
        var message = formatter(state, exception);
        Console.WriteLine($"[{correlationId}] {logLevel}: {message}");
    }
}

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            _logger.LogInformation("Handling request: {Path}", context.Request.Path);
            await _next(context);
            _logger.LogInformation("Finished handling request.");
        }
    }
}
