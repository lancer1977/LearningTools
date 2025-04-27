using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Configuration;

namespace SpellingTest.Core.Helpers
{
    public static class OidcClientHelper
    {
        public static OidcClient GetHelper(IConfiguration config, IBrowser browser)
        {
            var scopes = new List<string>{"openid", "profile", "api", "offline_access", "kona", "pathfinder", "documents",
                "collector", "campaign" };
            var client =  new OidcClient(new OidcClientOptions()
            {
                Authority = config["OIDC:Authority"],
                ClientSecret = config["OIDC:ClientSecret"],
                ClientId = config["OIDC:ClientId"],
                Scope = string.Join(" ", scopes),
                RedirectUri = config["OIDC:RedirectURI"],
                Browser = browser
            });
            return client;
        }
    }
}