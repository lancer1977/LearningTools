using PolyhydraGames.Learning.RestAsync;

namespace SpellingTest.Wasm.Setup;

public static class RestSetup
{
    public static IServiceCollection RegisterRest(this IServiceCollection builder, IConfiguration config)
    {
        builder.AddScoped(x => new LearningEndpointFactory(config["Endpoints:Learning"]));
        builder.AddScoped<ILearningFactory>(x => x.GetRequiredService<LearningEndpointFactory>());
        builder.AddScoped<IEndpointFactory>(x => x.GetRequiredService<LearningEndpointFactory>());
        return builder;
    }
}