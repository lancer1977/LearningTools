using PolyhydraGames.Core.Maui.Interfaces;
using SpellingTest.Core.ViewModels.Launcher;
using SpellingTest.Maui.Pages.Menu;

namespace SpellingTest.Maui.Pages.Root
{
    public class RootPage : PolyhydraGames.Core.Maui.Views.RootPageBase
    {
        protected override Type MenuType => typeof(MenuViewModel);

        protected override Type MainType => typeof(LauncherViewModel);

        public RootPage(IViewFactoryAsync viewFactory, IApp app) : base(viewFactory, app)
        {
        }

    }
}

