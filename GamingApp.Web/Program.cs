using System.Security.Claims;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.IdentityModel.JsonWebTokens;
using GamingApp.Web;
using GamingApp.Web.Components;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.FluentUI.AspNetCore.Components;
using GamingApp.Web.Clients;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");
builder.Services.AddFluentUIComponents();
builder.Services.AddDataGridEntityFrameworkAdapter();
builder.Services.AddHttpClient<UserApiClient>(client => client.BaseAddress = new Uri(builder.Configuration["backendapi"] ?? throw new InvalidOperationException()))
    .AddAuthToken();
builder.Services.AddHttpClient<GameApiClient>(client => client.BaseAddress = new Uri(builder.Configuration["backendapi"] ?? throw new InvalidOperationException()))
    .AddAuthToken();
builder.Services.AddHttpClient<CategoryApiClient>(client => client.BaseAddress = new Uri(builder.Configuration["backendapi"] ?? throw new InvalidOperationException()))
    .AddAuthToken();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.Redirect("/login");
                return Task.CompletedTask;
            }
        };
    })
    .AddOpenIdConnect(options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = builder.Configuration["IdentityUrl"];
        options.ClientId = "ClientWeb";
        options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email"); // Ensure this is included
        options.Scope.Add("ApiService");
        options.ClaimActions.MapJsonKey("name", "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email"); // Map the email claim explicitly
    });
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapPost("/user/signout", async (HttpContext httpContext, IAntiforgery antiforgery) =>
{
    await antiforgery.ValidateRequestAsync(httpContext);
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
}).AllowAnonymous();

await app.RunAsync();
