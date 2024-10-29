using Duende.IdentityServer;
using Duende.IdentityServer.Models;


public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("ApiService", "Api Service", ["role", "name", "email"])
    ];

    public static IEnumerable<Client> Clients(IConfiguration configuration)
    {
        return
        [
            new Client
            {
                ClientId = "ClientWeb",
                ClientName = "Client Web",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { $"{configuration["ClientWebEndpoint"]}/signin-oidc" },
                FrontChannelLogoutUri = $"{configuration["ClientWebEndpoint"]}/signout-oidc",
                PostLogoutRedirectUris = { $"{configuration["ClientWebEndpoint"]}/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "ApiService",
                    "role"
                }
            }
        ];
    }
}
