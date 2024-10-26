using Microsoft.IdentityModel.Tokens;
using GamingApp.ApiService.Data;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AspNetCoreRateLimit;
using GamingApp.ApiService;
using GamingApp.ApiService.Endpoints;
using GamingApp.ApiService.Services.Interfaces;
using GamingApp.ApiService.Services;

using GamingApp.ApiService.Extensions;
using FastEndpoints;

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

// Add global exception handling and structured logging with correlation IDs
builder.Services.AddLoggingExtensions();
builder.Services.AddExceptionMiddleware();

// Add dependency injection for better testability and maintainability
builder.Services.AddScoped<IAppDbContext, AppDbContext>();

// Add FastEndpoints services
builder.Services.AddFastEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseUserCheck();

// Add rate limiting middleware
app.UseIpRateLimiting();

// Add global exception handling middleware
app.UseExceptionMiddleware();

// Add structured logging middleware
app.UseLoggingMiddleware();

// Add FastEndpoints middleware
app.UseFastEndpoints();

await AppDbContext.EnsureDbCreatedAsync(app.Services);
await app.RunAsync();
