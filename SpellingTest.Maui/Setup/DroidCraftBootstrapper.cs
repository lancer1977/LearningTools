﻿using PolyhydraGames.Core.Maui.Services;
using PolyhydraGames.Learning.Interfaces;
using PolyhydraGames.Learning.RestAsync;
using PolyhydraGames.Learning.RestAsync.Services;
using SpellingTest.Core.Service;
using SpellingTest.Core.ViewModels.Quiz;

namespace SpellingTest.Maui.Setup;

public static class RestfulSetup
{
    public static MauiAppBuilder RegisterRestful(this MauiAppBuilder builder)
    {
        var webApiAddress = "https://api.polyhydragames.com/learning";
        builder.Services.AddSingleton<LearningEndpointFactory>(x => new LearningEndpointFactory(webApiAddress));
        builder.Services.AddSingleton<ILearningFactory>(x => x.GetRequiredService<LearningEndpointFactory>());
        builder.Services.AddSingleton<IEndpointFactory>(x => x.GetRequiredService<LearningEndpointFactory>()); 
        builder.Services.AddSingleton<IQuizService, QuizService>(); 
        builder.Services.AddSingleton<IAudioService, AudioService>();
        builder.Services.AddSingleton<IMathScoreService,SpeedMathService>();
        builder.Services.AddSingleton<ISettings, SettingService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<ISpellingNavigatorService, SpellingTestNavigatorService>();
        return builder;
    }
}