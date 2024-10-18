using Microsoft.IdentityModel.Tokens;
using GamingApp.ApiService.Data;
using GamingApp.ApiService.Endpoints;
using GamingApp.ApiService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
app.MapDefaultEndpoints();
await AppDbContext.EnsureDbCreatedAsync(app.Services);
await app.RunAsync();