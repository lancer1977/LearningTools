using PolyhydraGames.Extensions;
using PolyhydraGames.Extensions.Dice;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpellingTest.Core.ViewModels.Quiz
{
    public class FlashCardViewModel : ViewModelAsyncBase
    {
        [Reactive] public bool NextMode { get; set; }
        [Reactive] public bool UpsideDownMode { get; set; }
        [Reactive] public bool ShowName { get; set; }
        [Reactive] public bool ShowDefinition { get; set; }
        public ObservableAsPropertyHelper<string> _buttonText;
        public string ButtonText => _buttonText.Value;
        private readonly IDialogService _dialog;
        private readonly IQuizService _quizService;
        private readonly IAudioService _audioService;
        public ICommand ClickCommand { get; }
        [Reactive] public bool ScrollTop { get; set; }

        public FlashCardViewModel(IDialogService service, IQuizService quizService, IAudioService audioService)
        {
            _dialog = service;
            _quizService = quizService;
            _audioService = audioService;
            var notBusy = this.WhenAnyValue(vm => vm.IsBusy).Select(busy => !busy);
            _buttonText = this.WhenAnyValue(x => x.NextMode).Select(i => i ? "Next" : "Show")
                .ToProperty(this, v => v.ButtonText);
            {

            }
            ClickCommand = ReactiveCommand.Create(() =>
            {
                Answer();
                ScrollTop = NextMode;
            }, notBusy).OnExecuting(x => IsBusy = x);

            UpsideDownMode = true;
        }

        public void UpdateResults(Tuple<int, int> items)
        {
            var divisor = items.Item1 + items.Item2;
            if (divisor == 0) divisor = 1;
            var ratio = (double)items.Item1 / divisor;
            Results = $"Correct: {items.Item1}   Incorrect: {items.Item2} Ratio:{(int)(ratio * 100)}%";
            Debug.WriteLine("Done Updating Results");
        }

        private int _answered;
        private void Answer()
        {
            if (NextMode)
            {
                _answered += 1;
                if (_answered == Questions)
                {
                    _words = _words.Randomize();
                }

                ResetShow();
                Populate();



            }
            else
            {
                ShowName = ShowDefinition = true;
            }

            NextMode = !NextMode;
        }

        [Reactive] public string Results { get; set; }

        private void Populate()
        {
            var index = _answered % _words.Count();
            Word = _words[index];
        }

        [Reactive] public IDefinition Word { get; set; }

        private void ResetShow()
        {
            ShowName = !UpsideDownMode;
            ShowDefinition = UpsideDownMode;
        }
        private List<IDefinition> _words;
        public async Task LoadAsync(Guid model)
        {
            var id = model;
            var wordsResult = await _quizService.GetQuiz(id);
            var words = wordsResult.Definitions.ToList();
            if (words.Count < 3)
            {
                await _dialog.NotificationAsync("List is too short", "Error");
                return;
            }

            ResetShow();
            Title = wordsResult.Name;
            _words = words.Randomize();
            Debug.WriteLine("Words" + _words.Select(i => i.Name).Aggregate((x, y) => x + Environment.NewLine + y));
            Questions = _words.Count;
            Populate();
            await PlayMusic();
        }

        public void ResetWords()
        {

        }
        private async Task PlayMusic()
        {

            await _audioService.PlayMusic(DiceRoll.D4().ToString());
        }


        [Reactive] public int Questions { get; set; }

    }
}

