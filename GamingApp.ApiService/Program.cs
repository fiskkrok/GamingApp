using Microsoft.IdentityModel.Tokens;
using GamingApp.ApiService.Data;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AspNetCoreRateLimit;
using GamingApp.ApiService;
using GamingApp.ApiService.Endpoints;
using GamingApp.ApiService.Services.Interfaces;
using GamingApp.ApiService.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<AppDbContext>("apiservicedb");

// Add Redis caching configuration
builder.AddRedisClient("redis");
builder.AddRedisDistributedCache("redis");

// Add services to the container.
builder.Services.AddProblemDetails();
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityUrl"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOptions();
// Add rate limiting services
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost",
        name: "redis",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" })
    .AddDbContextCheck<AppDbContext>(name: "Database", failureStatus: HealthStatus.Degraded, tags: new[] { "ready" });

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapGameEndpoints();
app.MapUserEndpoints();
app.MapCategoryEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseUserCheck();

// Add rate limiting middleware
app.UseIpRateLimiting();

app.Use(async (context, next) =>
{
    var correlationId = Guid.NewGuid().ToString();
    context.Response.Headers.Add("X-Correlation-ID", correlationId);
    LogContext.PushProperty("CorrelationId", correlationId);

    var stopwatch = Stopwatch.StartNew();
    try
    {
        await next.Invoke();
    }
    finally
    {
        stopwatch.Stop();
        var logLevel = context.Response.StatusCode >= 500 ? LogEventLevel.Error : LogEventLevel.Information;
        Log.ForContext("ElapsedMilliseconds", stopwatch.ElapsedMilliseconds)
            .ForContext("StatusCode", context.Response.StatusCode)
            .Write(logLevel, "Request {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
});

app.MapDefaultEndpoints();
await AppDbContext.EnsureDbCreatedAsync(app.Services);
await app.RunAsync();
