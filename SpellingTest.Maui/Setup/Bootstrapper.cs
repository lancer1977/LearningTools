using Microsoft.AspNetCore.Authentication;
using PolyhydraGames.Core.Identity;
using PolyhydraGames.Core.Maui.Services;
using PolyhydraGames.Learning.Interfaces;
using PolyhydraGames.Learning.RestAsync;
using PolyhydraGames.Learning.RestAsync.Services;
using SpellingTest.Core;
using SpellingTest.Core.Interfaces;
using SpellingTest.Core.Service;
using SpellingTest.Core.ViewModels.Quiz;

namespace SpellingTest.Maui.Setup;

public static class RestfulSetup
{
    public static MauiAppBuilder RegisterRestful(this MauiAppBuilder builder)
    {

        builder.Services.AddSingleton(x => new LearningEndpointFactory(Constants.WebApiAddress));
        builder.Services.AddSingleton<ILearningFactory>(x => x.GetRequiredService<LearningEndpointFactory>());
        builder.Services.AddSingleton<IEndpointFactory>(x => x.GetRequiredService<LearningEndpointFactory>());
        builder.Services.AddSingleton<IQuizService, QuizService>();
        builder.Services.AddSingleton<IAudioService, AudioService>();
        builder.Services.AddSingleton<IMathScoreService, SpeedMathService>();
        builder.Services.AddSingleton<ISettings, SettingService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<ISpellingNavigatorService, SpellingTestNavigatorService>();
        builder.Services.AddSingleton<IBrowser>(x => Browser.Default);
        builder.Services.AddSingleton<ITextToSpeech>(x => TextToSpeech.Default);
        #if ANDROID || IOS || WINDOWS 
        builder.Services.AddSingleton<IdentityModel.OidcClient.Browser.IBrowser, MauiAuthenticationBrowser>();
        #else

        #endif
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<IAuthenticationClient>(x => x.GetRequiredService<AuthenticationService>());


        return builder;
    }
}