using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpellingTest.Core.Interfaces;
using SpellingTest.Core.Service;

namespace SpellingTest.Core.ViewModels.Launcher
{

    public class LauncherViewModel : ViewModelAsyncBase
    {
        [Reactive] public string Name { get; set; }
        [Reactive] public string DefinitionText { get; set; }
        public ICommand SaveNameCommand { get; }
        public ICommand TestCommand { get; }

        public LauncherViewModel(ISettingsService settings, IWebsiteRequestor requestor)
        {
            SaveNameCommand = ReactiveCommand.Create(() => settings.Name = Name);
            var command = ReactiveCommand.CreateFromTask(async () =>
           {
               await requestor.RequestWebsite(@"schoolToolsDefinition://github.com/lancer1977/DataSeeds/blob/master/Definitions/life.json");


               var testEntry = await IOC.Get<IDictionaryService>().GetAsync("Test");
               DefinitionText = testEntry;
           });
            command.ObserveOn(RxApp.MainThreadScheduler);
            TestCommand = command;
            Name = settings.Name;

        }

    }
}
