using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using PolyhydraGames.Extensions;
using PolyhydraGames.Extensions.Dice;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpellingTest.Core.ViewModels.Math;

namespace SpellingTest.Core.ViewModels.Quiz
{
    public class QuizViewModel : ViewModelAsyncBase
    {
        private readonly IQuizService _quizservice;
        private readonly IDialogService _dialog;
        private readonly IAudioService _audioService;
        public ICommand AnswerCommand { get; }
        [Reactive] public List<MathResult> Messages { get; set; } = new List<MathResult>();
        public char FeatureText => _featureText.Value;
        public readonly ObservableAsPropertyHelper<char> _featureText;
        private bool _isInvalid;


        public QuizViewModel(IQuizService quizservice, IDialogService service, IAudioService audioService)
        {
            _quizservice = quizservice;
            _dialog = service;
            _audioService = audioService;
            // Populate();
            // Feature = "+";
            var notBusy = this.WhenAnyValue(vm => vm.IsBusy).Select(busy => !busy);
            AnswerCommand = ReactiveCommand.CreateFromTask(async (string i) => await Answer(i), notBusy).OnExecuting(x => IsBusy = x);


            this.WhenAnyValue(vm => vm.Correct, vm => vm.Wrong, (x, y) => (x, y)).Subscribe((x) => UpdateResults(x.x, x.y));
        }

        public void UpdateResults(int item1, int item2)
        {
            var divisor = item1 + item2;
            if (divisor == 0) divisor = 1;
            var ratio = (double)item1 / divisor;
            Results = $"Correct: {item1}   Incorrect: {item2} Ratio:{(int)(ratio * 100)}%";
            Debug.WriteLine("Done Updating Results");
        }

        public override async Task OnAppearingAsync()
        {

            await base.OnAppearingAsync();
            if (_isInvalid)
                await IOC.Get<INavigator>().PopAsync();
        }
        private int _answered;
        private List<IDefinition> _words;

        private async Task<bool> Answer(string answer)
        {

            if (answer == Word.Name)
                NotifyCorrect(Word);
            else
                NotifyWrong(answer, Word);
            _answered += 1;
            Populate();

            if (_answered == Questions)
            {
                await _dialog.NotificationAsync(Results, "Congradulations");
            }


            return true;
        }

        [Reactive] public string Results { get; set; }
        private void NotifyCorrect(IDefinition model)
        {
            _audioService.PlaySound("Alright");
            Messages.Insert(0, MathResult.GetCorrectResult(model.Description, model.Name));
            Messages = Messages.ToList();


            //await _service.MsgBoxAsync("Correct");
            Correct += 1;
        }

        private void NotifyWrong(string wrong, IDefinition model)
        {
            try
            {
                _audioService.PlaySound("FloatingHead");
                Messages.Insert(0, MathResult.GetIncorrectResult(model.Description, wrong, model.Name));
                Messages = Messages.ToList();
            }
            catch (Exception ex)
            {

            }

            Wrong += 1;
        }

        private void Populate()
        {
            var index = _answered % _words.Count();
            Word = _words[index];
            var answerIndex = index;
            var wrongAnswer1 = GetWrongAnswerIndex(answerIndex, answerIndex);
            var wrongAnswer2 = GetWrongAnswerIndex(answerIndex, wrongAnswer1); ;
            PopulateText(Word.Name, _words[wrongAnswer1].Name, _words[wrongAnswer2].Name);
        }

        private int GetWrongAnswerIndex(int index, int index2)
        {
            var options = _words.Count;

            var roll = index;
            do
            {
                roll = DiceRoll.RollRandom(1, options) - 1;
            } while (roll == index || roll == index2);
            return roll;
        }

        private void PopulateText(string opt1, string opt2, string opt3)
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



        [Reactive] public IDefinition Word { get; set; }

        [Reactive] public string Option1Text { get; set; }

        [Reactive] public string Option2Text { get; set; }

        [Reactive] public string Option3Text { get; set; }


        public async Task LoadAsync(Guid model)
        {
            var quiz = await _quizservice.GetQuiz(model);
            var words = quiz.Definitions.ToList();
            if (words == null || words.Count < 3)
            {
                await _dialog.NotificationAsync("List is too short", "Error");
                return;
            }

            Title = quiz.Name;
            _words = words.Randomize();
            Debug.WriteLine("Words" + _words.Select(i => i.Name).Aggregate((x, y) => x + Environment.NewLine + y));
            Questions = _words.Count;
            Populate();
            await PlayMusic();
        }
        private async Task PlayMusic()
        {
            await _audioService.PlayMusic(DiceRoll.D4().ToString());
        }


        [Reactive] public int Questions { get; set; }

        [Reactive] public int Correct { get; set; }

        [Reactive] public int Wrong { get; set; }


    }
}

