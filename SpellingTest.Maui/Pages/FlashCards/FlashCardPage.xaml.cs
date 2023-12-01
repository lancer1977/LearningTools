using ReactiveUI;
using SpellingTest.Core.ViewModels.Quiz;

namespace SpellingTest.Maui.Pages.FlashCards
{
    public partial class FlashCardPage
    {
        public FlashCardPage()
        {
            InitializeComponent();
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var scrollchange = (BindingContext as FlashCardViewModel).WhenAnyValue(x => x.ScrollTop);
            System.IDisposable disposable = scrollchange.Subscribe(async x => await OnScroll(x));

        }

        private async Task OnScroll(bool x)
        {
            await Task.Delay(3);
            await scroll.ScrollToAsync(bottom, x ? ScrollToPosition.End : ScrollToPosition.Start, true);
        }
    }
}
