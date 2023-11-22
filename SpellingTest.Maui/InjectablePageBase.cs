using PolyhydraGames.Core.IOC;
using PolyhydraGames.Core.Maui.Controls;

namespace SpellingTest.Maui;

public class InjectablePageBase<T> : PageBase<T> where T : class, IViewModelAsync
{
    protected InjectablePageBase()
    {
        BindingContext = IOC.Get<T>();
    }
}