using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PolyhydraGames.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SpellingTest.Core.ViewModels.Math
{
    public class MathPickerViewModel : ViewModelAsyncBase
    {
        public MathPickerViewModel(INavigatorAsync nav)
        {
            StartCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await nav.PopPopupAsync();
                await nav.PushAsync<SpeedMathViewModel>(async x => await x.LoadAsync(Feature, Difficulty));
            });
            DifficultyOptions = EnumExtensions.EnumerateEnumType<Difficulty>().ToList();
            FeatureOptions = EnumExtensions.EnumerateEnumType<Feature>().ToList();
            QuestionOptions = new List<string> { "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
            Question = "10";
        }

        public ICommand StartCommand { get; }
        [Reactive] public List<Difficulty> DifficultyOptions { get; set; }
        [Reactive] public Difficulty Difficulty { get; set; }
        [Reactive] public List<Feature> FeatureOptions { get; set; }
        [Reactive] public Feature Feature { get; set; }
        [Reactive] public List<string> QuestionOptions { get; set; }
        [Reactive] public string Question { get; set; }
    }

}
