using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpellingTest.Core.Interfaces;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using PolyhydraGames.Core.Identity;

namespace SpellingTest.Core.ViewModels.Launcher
{

    public class LauncherViewModel : ViewModelAsyncBase
    {
        private readonly IAuthenticationClient _authClient;
        private readonly IQuizService _service;
        [Reactive] public string Name { get; set; } = "...";
        [Reactive] public string DefinitionText { get; set; } 
        public ICommand TestCommand { get; }

        public LauncherViewModel(ISettingsService settings, IWebsiteRequestor requestor, IAuthenticationClient authClient, IQuizService service)
        {
            _authClient = authClient;
            _service = service; 
            var command = ReactiveCommand.CreateFromTask(async () =>
           {
               try
               {
                   await _authClient.LoginAsync();
                   Name = _authClient.Claims.LastCFirst;
                   var result = await _service.GetFavorites();

                   foreach (var claim in result)
                   {
                       Debug.WriteLine(claim.Name);
                   }


               }
               catch (Exception ex)
               {
                   Debug.WriteLine(ex.Message);
               }

           });
            command.ObserveOn(RxApp.MainThreadScheduler);
            TestCommand = command; 

        }

    }
}
