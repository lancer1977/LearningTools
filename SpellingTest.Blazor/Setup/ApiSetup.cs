using PolyhydraGames.Learning.RestAsync;

namespace SpellingTest.Web.Setup;

public static class RestSetup
{
    public static IHostApplicationBuilder RegisterRest(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped(x => new LearningEndpointFactory(builder.Configuration["Endpoints:Learning"]));
        builder.Services.AddScoped<ILearningFactory>(x => x.GetRequiredService<LearningEndpointFactory>());
        builder.Services.AddScoped<IEndpointFactory>(x => x.GetRequiredService<LearningEndpointFactory>());
        return builder;
    }
} 