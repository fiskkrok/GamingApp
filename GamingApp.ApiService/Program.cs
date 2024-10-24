using Microsoft.IdentityModel.Tokens;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Endpoints;
using GamingApp.ApiService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AspNetCoreRateLimit;
using GamingApp.ApiService.Services.Interfaces;
using GamingApp.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<AppDbContext>("apiservicedb");

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

app.MapDefaultEndpoints();
await AppDbContext.EnsureDbCreatedAsync(app.Services);
await app.RunAsync();
