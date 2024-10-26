using Microsoft.IdentityModel.Tokens;
using GamingApp.ApiService.Data;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AspNetCoreRateLimit;
using FluentValidation;
using FluentValidation.AspNetCore;
using GamingApp.ApiService;
using GamingApp.ApiService.Services.Interfaces;
using GamingApp.ApiService.Services;

using GamingApp.ApiService.Extensions;
using FastEndpoints;

using GamingApp.ApiService.Validation;

using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<AppDbContext>("apiservicedb");

// Add Redis caching configuration
builder.AddRedisClient("redis");
builder.AddRedisDistributedCache("redis");

// Add request size limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024; // 1MB
    options.ValueLengthLimit = 1024 * 1024; // 1MB
    options.MemoryBufferThreshold = 1024 * 1024; // 1MB
});
// Configure Kestrel server limits
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024; // 1MB
    options.Limits.MaxRequestHeadersTotalSize = 32 * 1024; // 32KB
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(60);
});
// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserProfileValidator>();
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
builder.Services.AddLoggingExtensions(); // Ensure this method is defined in the GamingApp.ApiService.Logging namespace
//builder.Services.AddExceptionMiddleware();

// Add dependency injection for better testability and maintainability
builder.Services.AddScoped<AppDbContext>();

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
