using System.Diagnostics;
using PolyhydraGames.Core.IOC;
using PolyhydraGames.Core.Maui.Interfaces;
using PolyhydraGames.Core.ReactiveUI;
using PolyhydraGames.Learning.Dtos;
using PolyhydraGames.Learning.Interfaces;
using SpellingTest.Core.ViewModels.CardGame;
using SpellingTest.Core.ViewModels.Launcher;
using SpellingTest.Core.ViewModels.Math;
using SpellingTest.Core.ViewModels.Quiz;
using SpellingTest.Core.ViewModels.Scores;
using MenuItem = PolyhydraGames.Core.ReactiveUI.MenuItem;
namespace SpellingTest.Maui.Pages.Menu
{
    public class MenuViewModel : ViewModelAsyncBase
    {
        private IApp _app;
        private void HideMenu()
        {
            if (_app.MainPage is FlyoutPage master)
            {
                master.IsPresented = false;
            }
        }

        private readonly INavigatorAsync _nav; 
        private readonly IMathScoreService _result;

        public MenuViewModel(INavigatorAsync nav, IApp app, IMathScoreService result)
        {
            MenuItem.Initialize(nav,IOC.Get<IMenuControl>(),IOC.Get<IMainThreadDispatcher>());
            _nav = nav;
            _app = app;
            _result = result;

            MathMenuItem =  new  MenuItem("Math Quiz", OnCreation);
            SpellingListMenuItem = MenuItem.Create<QuizListPickerViewModel>("Spelling List");
            CardGameMenuItem = MenuItem.Create<CardGameViewModel>("Card Game",async x=> await x.LoadAsync(Guid.Parse( "91F9423E-F36B-1410-8F2B-00F968453034")));
            ScoreMenuItem = MenuItem.Create<ScoreViewModel>("Scores");
            SettingsMenuItem = MenuItem.Create<LauncherViewModel>("Settings");
            TestMenuItem = MenuItem.Create("Insert Test", InsertAction);
        }

        private async Task OnCreation()
        {
            await MathAction();
        }

        public MenuItem MathMenuItem { get; }
        public MenuItem ScoreMenuItem { get; }
        public MenuItem SpellingListMenuItem { get; }
        public MenuItem CardGameMenuItem { get; }
        public MenuItem TestMenuItem { get; }
        public MenuItem SettingsMenuItem { get; }

        private async Task MathAction()
        {
            try
            {
                await _nav.PushPopupAsync<MathPickerViewModel>(null );
                HideMenu();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private async Task InsertAction()
        {
            await _result.Insert(new SpeedMathResult()
            {
                Date = DateTime.Now,
                Difficulty = Difficulty.Easy,
                Name = "Chris",
                Operation = Feature.Add,
                Questions = 100,
                Seconds = 117
            });
        }
     
        public override string Title => "Main Menu";
        
    }
}

