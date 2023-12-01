
using SpellingTest.Core.ViewModels.Math;

namespace SpellingTest.Maui.Pages.SpeedMath
{
    public partial class SpeedMathPage
    {
        public SpeedMathPage(SpeedMathViewModel vm)
        {
            BindingContext = vm;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            answerBox.Focus();
        }
    }
}
