using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting; 
using PolyhydraGames.Core.Maui.Setup;
using SpellingTest.Core.ViewModels.Quiz;
using SpellingTest.Maui.Setup;

namespace SpellingTest.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var assembly = typeof(MauiProgram).Assembly;
            var builder = MauiApp.CreateBuilder();
            var viewAssemblies = new[] { assembly, typeof(QuizListPickerViewModel).Assembly, typeof(DeviceModule).Assembly};
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() 
                .RegisterPlatformServices()
                //.RegisterTypes(PathfinderRulesBootstrapper.GetRegistrationTypes())
                .RegisterIOC() 
                .RegisterMauiCoreServices()
                .ConfigureMopups()
                .RegisterViewModelsAndPages(viewAssemblies) 
                .UseMauiCommunityToolkit()
                .RegisterRestful()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            var build =  builder.Build();

            build.Services.GetRequiredService<IIOCContainer>(); 
            ViewModelModule.ViewFactoryRegistration(viewAssemblies);
            return build;
        }
    }
}