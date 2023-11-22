using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SpellingTest.Web.Setup;

public static class AuthSetup
{
    public static void AddOIDC(this WebApplicationBuilder builder)
    {

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnSigningOut = async e =>
                {
                    // revoke refresh token on sign-out
                    //await e.HttpContext.RevokeUserRefreshTokenAsync();
                };
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                // Set Authority to setting in appsettings.json.  This is the URL of the IdentityServer4
                options.UsePkce = true;
                options.Authority = builder.Configuration["OIDC:Authority"];
                options.ClientId = builder.Configuration["OIDC:ClientId"];
                options.ClientSecret = builder.Configuration["OIDC:ClientSecret"];
                // When set to code, the middleware will use PKCE protection
                options.ResponseType = "code";
                // Add request scopes.  The scopes are set in the Client >  Basic tab in IdentityServer Admin UI
                options.Scope.Add("openid");
                //options.Scope.Add("profile");

                //options.Scope.Add("documents");
                //options.Scope.Add("email");
                options.Scope.Add("kona");
                options.Scope.Add("pathfinder");
                options.Scope.Add("documents");
                options.Scope.Add("collector");
                options.Scope.Add("campaign");
                //options.Scope.Add("roles");
                //options.Scope.Add("profile");
                //options.Scope.Add("verification");

                ////enables refresh tokens
                //options.Scope.Add("offline_access");
                options.ClaimActions.MapJsonKey("email_verified", "email_verified");
                options.ClaimActions.MapJsonKey("id", "id");
                options.GetClaimsFromUserInfoEndpoint = true;


                // Save access and refresh tokens to authentication cookie.  the default is false
                options.SaveTokens = true;
                // It's recommended to always get claims from the 
                // UserInfoEndpoint during the flow. 
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //map claim to name for display on the upper right corner after login.  Can be name, email, etc.
                    NameClaimType = "name"
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnAccessDenied = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                        return Task.CompletedTask;
                    },

                };
            });
    }
}