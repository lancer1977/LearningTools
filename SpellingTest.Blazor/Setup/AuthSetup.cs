using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PolyhydraGames.Core.Interfaces;

namespace SpellingTest.Web.Setup;

public static class AuthSetup
{
    public static void AddOIDC(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
            .AddCookie("cookie", options =>
            {
                options.Cookie.Name = "__Host-blazor";
                options.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddOpenIdConnect("oidc", options =>
            { 
                options.Authority = builder.Configuration["OIDC:Authority"];
                options.ClientSecret = builder.Configuration["OIDC:ClientSecret"];
                //options.ClientId = "interactive.confidential";
                options.ClientId = builder.Configuration["OIDC:ClientId"];
                options.ResponseType = "code";
                options.ResponseMode = "query";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("api");
                options.Scope.Add("offline_access");
                options.Scope.Add("kona");
                options.Scope.Add("pathfinder");
                options.Scope.Add("documents");
                options.Scope.Add("collector");
                options.Scope.Add("campaign");
                options.MapInboundClaims = false;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
            });
             
    }
}