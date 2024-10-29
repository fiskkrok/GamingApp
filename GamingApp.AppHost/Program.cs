using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("PostgresPassword", true);
var postgresServer = builder
    .AddPostgres("GamingApp-postgres", password: dbPassword).WithPgWeb().WithDataVolume();
var apiserviceDb = postgresServer
    .AddDatabase("apiservicedb");

var identityServer = builder.AddProject<IdentityServerdaw>("identityserver").WithExternalHttpEndpoints();
var identityEndpoint = identityServer
    .GetEndpoint("https");
var redis = builder.AddRedis("redis").WithRedisInsight();

var apiService = builder.AddProject<GamingApp_ApiService>("apiservice")
    .WithReference(apiserviceDb)
    .WaitFor(apiserviceDb)
    .WithEnvironment("IdentityUrl", identityEndpoint)
    .WithReference(redis); // Utilize Redis setup for caching in the API service

var clientweb = builder.AddProject<GamingApp_Web>("clientweb")
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("IdentityUrl", identityEndpoint);

identityServer.WithEnvironment("ClientWebEndpoint", clientweb.GetEndpoint("https"));

await builder.Build().RunAsync();
