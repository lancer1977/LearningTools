using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using PolyhydraGames.BlazorComponents.Dialog;
using PolyhydraGames.Core.AspNet.OwnerMiddleware;
using PolyhydraGames.Learning.Interfaces;
using PolyhydraGames.Learning.RestAsync.Services;
using PolyhydraGames.SignalR.Service;
using SpellingTest.Core.Interfaces;
using SpellingTest.Core.Service;
using SpellingTest.Web.Services;
using SpellingTest.Web.Services.CurrentPage;
using SpellingTest.Web.Services.Fakes;
using SpellingTest.Web.Services.Wrappers;

namespace SpellingTest.Web.Setup;

public static class ApiRegistrationHelper
{

    public static void AddBlazorise(this IServiceCollection services)
    {
        services.AddBlazorise(options => { options.Immediate = true; }).AddBootstrapProviders().AddFontAwesomeIcons();
    }

    public static void AddMiscServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IWebsiteRequestor, WebsiteRequestorFake>();
        builder.Services.AddScoped<ISettings, SettingsFake>();
        builder.Services.AddScoped<ISettingsService, SettingsService>();
        builder.Services.AddScoped<IHttpService, HttpFinder>();
        builder.Services.AddScoped<IDialogService, DialogService>();
        builder.Services.AddScoped<DialogService>();

        builder.Services.AddScoped<IJSHelper, ClientInteroperability>();
        builder.Services.AddScoped<IChatService>(x =>
            new ChatService(SignalRHelpers.Create(builder.Configuration["Endpoints:Website"], "chathub")));
        builder.Services.AddScoped<ICurrentPage, CurrentPageService>();
        builder.Services.AddScoped<ISpellingNavigatorService, NavigationHelper>();
        
        builder.Services.AddScoped<IOwnerService, OwnerService>();
        builder.Services.AddScoped<IQuizService, QuizService>();
        builder.Services.AddScoped<IMathScoreService, SpeedMathService>();
        builder.Services.AddScoped<INavigatorAsync, NavigatorServiceFake>();
        builder.Services.AddScoped<IAudioService, AudioServiceFake>();
        builder.Services.AddScoped<IMainThreadDispatcher, MainThreadDispatcher>();
    }

    public static void AddCors(this WebApplicationBuilder collection)
    {
        collection.Services.AddCors(options =>

            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins("https://app.roll20.net").AllowCredentials();
                builder.AllowAnyMethod().AllowAnyHeader()
                    .WithOrigins("http://localhost:55830",
                        "https://localhost:44368/",
                        "https://polyplayground.azurewebsites.net",
                        "https://polyhydragames.azurewebsites.net",
                        "https://polyhydragames.com",
                        "https://demo.duendesoftware.com",
                        "https://demo.duendesoftware.com",
                        "https://polyhydragames.asuscomm.com",
                        "https://identity.polyhydragames.com",
                        "https://blazor.polyhydragames.com")
                    .AllowCredentials();

            }));
    }
}