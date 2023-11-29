using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpellingTest.Core.Interfaces;

namespace SpellingTest.Core.ViewModels.Launcher
{

    public class LauncherViewModel : ViewModelAsyncBase
    {
        private readonly IAuthenticationClient _authClient;
        private readonly IQuizService _service;
        [Reactive] public string Name { get; set; }
        [Reactive] public string DefinitionText { get; set; }
        public ICommand SaveNameCommand { get; }
        public ICommand TestCommand { get; }

        public LauncherViewModel(ISettingsService settings, IWebsiteRequestor requestor, IAuthenticationClient authClient, IQuizService service)
        {
            _authClient = authClient;
            _service = service;
            SaveNameCommand = ReactiveCommand.Create(() => settings.Name = Name);
            var command = ReactiveCommand.CreateFromTask(async () =>
           {
               try
               {
                   await _authClient.LoginAsync();
                   var result = await _service.GetFavorites();
                   foreach (var VARIABLE in result)
                   { 
                       Debug.WriteLine(VARIABLE.Name);
                   }


               }
               catch (Exception ex)
               {
                   Debug.WriteLine(ex.Message);
               }

           });
            command.ObserveOn(RxApp.MainThreadScheduler);
            TestCommand = command;
            Name = settings.Name;

        }

    }
}
