using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using wallet.api.Features.Core;

namespace wallet.api.Features.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: "wallet", displayName: "wallet api")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "wallet",
                ClientName = "wallet-api",
                AccessTokenLifetime = 60 * 60 * 24,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("super-secret-key".Sha256()) },
                RedirectUris = { "https://localhost:7001/signin-oidc" },
                AlwaysIncludeUserClaimsInIdToken = true,
                PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },
                AllowOfflineAccess = true,

                AllowedScopes =
                {
                    "openid",
                    "profile",
                    "wallet"
                }
            }
        };

    internal static IEnumerable<ApiScope> GetApiScopes()
    {
        return new[]
        {
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
            new ApiScope
            (
                "wallet",
                "openid",
                new[] { JwtClaimTypes.Name, JwtClaimTypes.Role, JwtClaimTypes.Email, JwtClaimTypes.Id, Consts.Sub}
            )
        };
    }
}
