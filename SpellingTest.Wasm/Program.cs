using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PolyhydraGames.BlazorComponents;
using PolyhydraGames.Core.Identity;
using SpellingTest.Core;
using SpellingTest.Core.Helpers;
using SpellingTest.Wasm;
using SpellingTest.Wasm.Setup;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorise();
builder.Services.AddBlazorComponents();
builder.Services.AddHttpClient();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<IAuthenticationClient, ClientSideIdentityService>();
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();


builder.Services.AddMiscServices(builder.Configuration);
builder.Services.RegisterRest(builder.Configuration);
builder.Services.RegisterViewModels();

builder.Services.AddSingleton(x => OidcClientHelper.GetHelper(x.GetService<IConfiguration>(), x.GetRequiredService<IBrowser>()));

await builder.Build().RunAsync();
