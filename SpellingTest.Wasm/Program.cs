using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PolyhydraGames.BlazorComponents;
using SpellingTest.Wasm;
using SpellingTest.Wasm.Setup;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorise();
builder.Services.AddBlazorComponents();  
builder.Services.AddSyncfusionBlazor();  
builder.Services.AddHttpClient();
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();


builder.Services.AddMiscServices(builder.Configuration);
builder.Services.RegisterRest(builder.Configuration);
builder.Services.RegisterViewModels();



await builder.Build().RunAsync();
