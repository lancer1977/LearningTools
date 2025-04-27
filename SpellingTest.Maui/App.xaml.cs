using PolyhydraGames.Core.IOC;
using PolyhydraGames.Core.Maui;
using PolyhydraGames.Core.Maui.Interfaces;
using PolyhydraGames.Core.Maui.Services;
using PolyhydraGames.Core.Maui.Views;

namespace SpellingTest.Maui;

public partial class App : ShellApplication
{
    public App()
    {
        InitializeComponent();

        MainPage = new ContentPage();

    }


    protected override async void OnStart()
    {
        //_navigationBarColor = Color.FromHex(ColorManager.Settings.GetValueOrDefault(Keys.SplashColor, "#00000000"));
        //ColorManager.LoadColors(this);
        {
            base.OnStart();
            MainPage = await IOC.Get<IViewFactoryAsync>().ResolveAsync<RootViewModel>();
        }
    }
}
