using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using PolyhydraGames.Blazor.Data.CurrentPage;
using PolyhydraGames.BlazorComponents.CurrentPage;
using PolyhydraGames.BlazorComponents.Dialog;
using PolyhydraGames.Core.Interfaces;
using PolyhydraGames.Learning.Interfaces;
using PolyhydraGames.Learning.RestAsync.Services;
using PolyhydraGames.SignalR.Service;
using SpellingTest.Core.Interfaces;
using SpellingTest.Core.Service;
using SpellingTest.Wasm.OwnerMiddleware;
using SpellingTest.Wasm.Services;
using SpellingTest.Wasm.Services.Fakes;
using SpellingTest.Wasm.Services.Wrappers;

namespace SpellingTest.Wasm.Setup;

public static class ApiRegistrationHelper
{

    public static void AddBlazorise(this IServiceCollection services)
    {
        services.AddBlazorise(options => { options.Immediate = true; }).AddBootstrapProviders().AddFontAwesomeIcons();
    }

    public static void AddMiscServices(this IServiceCollection builder, IConfiguration config)
    {
        builder.AddScoped<IWebsiteRequestor, WebsiteRequestorFake>();
        builder.AddScoped<ISettings, SettingsFake>();
        builder.AddScoped<ISettingsService, SettingsService>();
        builder.AddScoped<IHttpService, HttpFinder>();
        builder.AddScoped<ITextToSpeech, TextToSpeechFake>();
        builder.AddScoped<IDialogService, DialogService>();
        builder.AddScoped<DialogService>();

        builder.AddScoped<IJSHelper, ClientInteroperability>();
        builder.AddScoped<IChatService>(x =>
            new ChatService(SignalRHelpers.Create(config["Endpoints:Website"], "chathub")));
        builder.AddScoped<ICurrentPage, CurrentPageService>();
        builder.AddScoped<ISpellingNavigatorService, NavigationHelper>();
        
        builder.AddScoped<IOwnerService, OwnerService>();
        builder.AddScoped<IQuizService, QuizService>();
        builder.AddScoped<IMathScoreService, SpeedMathService>();
        builder.AddScoped<INavigatorAsync, NavigatorServiceFake>();
        builder.AddScoped<IAudioService, AudioServiceFake>();
        builder.AddScoped<IMainThreadDispatcher, MainThreadDispatcher>();
    }

 
}