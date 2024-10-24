using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("PostgresPassword", true);
var postgresServer = builder
    .AddPostgres("GamingApp-postgres", password: dbPassword).WithPgWeb();
var apiserviceDb = postgresServer
    .AddDatabase("apiservicedb");
var identityServer = builder.AddProject<IdentityServer>("identityserver").WithExternalHttpEndpoints();
var identityEndpoint = identityServer
    .GetEndpoint("https");
var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<GamingApp_ApiService>("apiservice")
    .WithReference(apiserviceDb)
    .WithEnvironment("IdentityUrl", identityEndpoint)
    .WithReference(cache); // Utilize Redis setup for caching in the API service

var clientweb = builder.AddProject<GamingApp_Web>("clientweb")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WithEnvironment("IdentityUrl", identityEndpoint);

identityServer.WithEnvironment("ClientWebEndpoint", clientweb.GetEndpoint("https"));

await builder.Build().RunAsync();
