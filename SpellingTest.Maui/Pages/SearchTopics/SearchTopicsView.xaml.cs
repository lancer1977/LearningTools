using SpellingTest.Core.ViewModels.SearchTopics;

namespace SpellingTest.Maui.Pages.SearchTopics;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SearchTopicsPage : InjectablePageBase<SearchTopicsViewModel>
{
    public SearchTopicsPage()
    {
        InitializeComponent();
    }
}
