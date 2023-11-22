using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PolyhydraGames.Core.Global.Properties;
using PolyhydraGames.Extensions;
using PolyhydraGames.Extensions.Dice;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SpellingTest.Core.ViewModels.Math
{
    public class MathViewModel : ViewModelAsyncBase
    {
        private readonly IDialogService _service;
        private readonly IAudioService _audioService;
        private readonly IMainThreadDispatcher _mainThreadDispatcher;
        public ICommand AnswerCommand { get; }
        public ObservableCollection<MathResult> Messages { get; } = new();
        [ObservableAsProperty] public char FeatureText { get; }

        [Reactive] public int Number1 { get; set; }

        [Reactive] public int Number2 { get; set; }


        [Reactive] public Feature Feature { get; set; }

        [Reactive] public int Option1Text { get; set; }

        [Reactive] public int Option2Text { get; set; }

        [Reactive] public int Option3Text { get; set; }


        [Reactive] public int Questions { get; set; }
        [Reactive] public Difficulty Difficulty { get; set; }
        [Reactive] public int Correct { get; set; }
        [Reactive] public int Wrong { get; set; }

        private bool _isLoaded;



        public MathViewModel(IDialogService service, IAudioService audioService, IMainThreadDispatcher mainThreadDispatcher)
        {
            _service = service;
            _audioService = audioService;
            _mainThreadDispatcher = mainThreadDispatcher;
            // Populate();
            // Feature = "+";
            var notBusy = this.WhenAnyValue(vm => vm.IsBusy).Select(busy => !busy);
            AnswerCommand = ReactiveCommand.CreateFromTask<int>(AnswerAction, notBusy).OnExecuting(x => IsBusy = x);
             this.WhenAnyValue(vm => vm.Feature).Select(f => f.ToChar()).ToPropertyEx(this, x => x.FeatureText);
            this.WhenAnyValue(vm => vm.Feature).Subscribe(UpdateTitle);
            this.WhenAnyValue(vm => vm.Correct, vm => vm.Wrong).Subscribe((x) => UpdateResults((x.Item1, x.Item2)));
        }

        private async Task AnswerAction([NotNull] int i)
        {
            switch (i)
            {
                case 1:
                    await Answer(Option1Text);
                    break;

                case 2:
                    await Answer(Option2Text);
                    break;

                case 3:
                    await Answer(Option3Text);
                    break;
            }
        }

        public void UpdateResults((int part1, int part2) items)
        {
            var divisor = items.part1 + items.part2;
            if (divisor == 0) divisor = 1;
            double ratio = (double)items.part1 / divisor;
            Results = $"Correct: {items.part1}   Incorrect: {items.part2} Ratio:{(int)(ratio * 100)}%";
            Debug.WriteLine("Done Updating Results");
        }

        private void UpdateTitle(Feature feature)
        {
            Title = feature.ToString();
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();  
            await LoadAsync(Feature.Add, -1, Difficulty.Easy);
        }
        private int _answered;
        private async Task<bool> Answer(int answer)
        {
            Func<int, int, int, bool> function = null;
            Func<int, int, int> getCorrectAmount = null;
            switch (Feature)
            {
                case Feature.Multiply:
                    getCorrectAmount = (n1, n2) => n1 * n2;
                    break;
                case Feature.Divide:
                    getCorrectAmount = (n1, n2) => n1 / n2;
                    break;
                case Feature.Add:
                    getCorrectAmount = (n1, n2) => n1 + n2;
                    break;
                case Feature.Subtract:
                    getCorrectAmount = (n1, n2) => n1 - n2;
                    break;
                default:
                    NotifyError(answer);
                    return false;
            }
            function = (n1, n2, a) => getCorrectAmount(n1, n2) == a;
            if (function.Invoke(Number1, Number2, answer))
                await NotifyCorrect(answer);
            else
                await NotifyWrong(answer, getCorrectAmount(Number1, Number2));
            Populate();
            _answered += 1;
            if (_answered == Questions)
            {
                await _service.NotificationAsync(Results, "Congradulations");
                var destination = (int)Feature == 3 ? Feature.Add : Feature + 1;
                await IOC.Get<INavigatorAsync>().NavigateTo<MathViewModel>(async i => await i.LoadAsync(Feature + 1, Questions, Difficulty));
            }


            return true;
        }

        [Reactive] public string Results { get; set; }
        private async Task NotifyCorrect(int answer)
        {
            await _audioService.PlaySound("Alright");
            _mainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Messages.Insert(0, MathResult.GetCorrectResult(Number1.ToString() + Feature.ToChar() + Number2, answer.ToString()));
            });
            Correct += 1;
        }

        private async Task NotifyWrong(int answer, int rightAnswer)
        {
            await _audioService.PlaySound("FloatingHead");
            _mainThreadDispatcher.InvokeOnMainThread(() =>
            {
                Messages.Insert(0, MathResult.GetIncorrectResult(Number1.ToString() + Feature.ToChar() + Number2, answer.ToString(), rightAnswer.ToString()));
            });
            Wrong += 1;
        }

        private void NotifyError(int anwswer)
        {
            _service.NotificationAsync("Error");
        }
        private void Populate()
        {
            switch (Feature)
            {
                case Feature.Add:
                    PopulateAddition();
                    break;
                case Feature.Subtract:
                    PopulateSubtraction();
                    break;
                case Feature.Divide:
                    PopulateDivision();
                    break;
                case Feature.Multiply:
                    PopulateMulti();
                    break;
                default:
                    NotifyError(1);
                    break;
            }

        }

        private void PopulateSubtraction()
        {
            Number1 = DiceRoll.RollRandom(0, 10);
            Number2 = DiceRoll.RollRandom(0, 10);
            var answer = Number1 - Number2;
            var wrongAnswer1 = GetWrongAnswer(answer);
            var wrongAnswer2 = GetWrongAnswer(answer); ;
            PopulateText(answer, wrongAnswer1, wrongAnswer2);
        }

        private int GetWrongAnswer(int answer)
        {
            return answer + (DiceRoll.RollRandom(0, 1).ToBool() ? DiceRoll.D10() : (DiceRoll.D10() * -1));
        }
        private void PopulateAddition()
        {
            Number1 = DiceRoll.RollRandom(0, 10);
            Number2 = DiceRoll.RollRandom(0, 10);
            var answer = Number1 + Number2;
            var wrongAnswer1 = GetWrongAnswer(answer);
            var wrongAnswer2 = GetWrongAnswer(answer); ;
            PopulateText(answer, wrongAnswer1, wrongAnswer2);
        }

        private void PopulateDivision()
        {
            Number2 = DiceRoll.RollRandom(1, 10);
            Number1 = DiceRoll.RollRandom(1, 10) * Number2;
            Debug.WriteLine($"Number2: {Number2} Number1: {Number1}");
            var answer = Number1 / Number2;
            var wrongAnswer1 = GetWrongAnswer(answer);
            var wrongAnswer2 = GetWrongAnswer(answer); ;
            PopulateText(answer, wrongAnswer1, wrongAnswer2);
        }

        private void PopulateMulti()
        {
            Number1 = DiceRoll.RollRandom(0, 10);
            Number2 = DiceRoll.RollRandom(0, 10);
            var answer = Number1 * Number2;
            var wrongAnswer1 = GetWrongAnswer(answer);
            var wrongAnswer2 = GetWrongAnswer(answer); ;
            PopulateText(answer, wrongAnswer1, wrongAnswer2);
        }

        private void PopulateText(int opt1, int opt2, int opt3)
        {
            switch (DiceRoll.RollRandom(1, 3))
            {
                case 1:
                    Option1Text = opt1;
                    Option2Text = opt2;
                    Option3Text = opt3;
                    break;
                case 2:
                    Option1Text = opt3;
                    Option2Text = opt1;
                    Option3Text = opt2;
                    break;
                case 3:
                    Option1Text = opt2;
                    Option2Text = opt3;
                    Option3Text = opt1;
                    break;
            }
        }
         
        public async Task LoadAsync(Feature process, int questions, Difficulty dif)
        {
            if (_isLoaded) return;
            _isLoaded = true;
            Feature = process;
            Questions = questions;
            Difficulty = dif;
            Populate();
            await PlayMusic();
        }

        private async Task PlayMusic()
        {
            switch (Feature)
            {
                case Feature.Add:
                    await _audioService.PlayMusic("1");
                    break;
                case Feature.Subtract:
                    await _audioService.PlayMusic("2");
                    break;
                case Feature.Divide:
                    await _audioService.PlayMusic("3");
                    break;
                case Feature.Multiply:
                    await _audioService.PlayMusic("4");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
