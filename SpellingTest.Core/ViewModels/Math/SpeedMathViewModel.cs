using SpellingTest.Core.Helpers;

namespace SpellingTest.Core.ViewModels.Math
{
    public class SpeedMathConfig
    {
        public int Count { get; }
        public Difficulty Difficulty { get; }
        public Feature Feature { get; }
        private int LowerRange { get; }
        private int HighRange { get; }

        public SpeedMathConfig(int count, Difficulty difficulty, Feature feature)
        {
            Count = count;
            Difficulty = difficulty;
            Feature = feature;
            LowerRange = SpeedMathHelper.GetLower(difficulty);
            HighRange = SpeedMathHelper.GetUpper(difficulty);
        }

        public List<MathQuestion> GetQuestionss()
        {
            var items = SpeedMathHelper.PopulateQuestions(Feature, LowerRange, HighRange).GetRandomizedList();
            return items.GetRange(0, Count);
        }
    }
    public class SpeedMathViewModel : ViewModelAsyncBase, IDisposable
    {
        private int QuestionsCount { get; set; }
        private Queue<MathQuestion> Questions { get; set; }
        private readonly IDialogService _service;
        private readonly IAudioService _audioService;
        private readonly ISettingsService _settings;
        private readonly IMathScoreService _mathScoreService;
        private double _ellapsedSeconds;
        private int _answered;
        private bool _isLoaded;
        [Reactive] public MathQuestion CurrentQuestion { get; private set; }
        [Reactive] public string Results { get; set; }

        [Reactive] public Difficulty Difficulty { get; set; }
        [Reactive] public int Correct { get; set; }
        [Reactive] public Feature Feature { get; set; }
        [Reactive] public string Answer { get; set; }
        public char FeatureText => _featureText.Value;
        private readonly ObservableAsPropertyHelper<char> _featureText;
        public DateTime StartTime { get; set; }


        public SpeedMathViewModel(IDialogService service, IAudioService audioService, ISettingsService settings, IMathScoreService mathScoreService)
        {
            _service = service;
            _audioService = audioService;
            _settings = settings;
            _mathScoreService = mathScoreService;
            var notBusy = this.WhenAnyValue(vm => vm.IsBusy).Select(busy => !busy);

            _featureText = this.WhenAnyValue(vm => vm.Feature).Select(f => f.ToChar()).ToProperty(this, x => x.FeatureText);
            this.WhenAnyValue(vm => vm.Feature).ObserveOn(RxApp.MainThreadScheduler).Subscribe(UpdateTitle);
            this.WhenAnyValue(vm => vm.Correct).Skip(1).Subscribe(UpdateResults);
            this.WhenAnyValue(vm => vm.Answer).Select(OnAnswerChanged).Subscribe();
        }
        public async Task LoadAsync(SpeedMathConfig config)
        {
            if (_isLoaded) return;
            try
            {
                // 

                StartTime = DateTime.Now;
                _isLoaded = true;
                Feature = config.Feature;
                Difficulty = config.Difficulty;
                Questions = new Queue<MathQuestion>(config.GetQuestionss());
                QuestionsCount = Questions.Count;
                Populate();
                await PlayMusic();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// user inputs text
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task OnAnswerChanged(string answer)
        {
            if (string.IsNullOrEmpty(answer)) return;
            var correct = TestAnswer(answer.ToInt());
            if (correct)
            {
                await _audioService.PlaySound("Alright");
                _answered += 1;
                Correct += 1;
                if (_answered == QuestionsCount)
                {
                    await RunWinScenario();
                    return;
                }
                Populate();
                Answer = string.Empty;
            }

        }

        /// <summary>
        /// Runs when the items are all answered
        /// </summary>
        /// <returns></returns>
        private async Task RunWinScenario()
        {
            await _mathScoreService.Insert(new PolyhydraGames.Learning.Dtos.SpeedMathResult()
            {
                Date = DateTime.Now,
                Seconds = _ellapsedSeconds,
                Operation = Feature,
                Difficulty = Difficulty,
                Questions = QuestionsCount,
                Name = _settings.Name
            });
            var response = (await _service.GetBooleanAsync(Results, "Congratulations", "Start Another?", "End")).Result;

            if (response)
            {
                await StartFreshGameAsync();
            }
        }

        private async Task StartFreshGameAsync()
        {
            StartTime = DateTime.Now;
            Correct = 0;
            _answered = 0;

            Populate();
            await PlayMusic();
        }
        public void UpdateResults(int items)
        {
            try
            {
                var ellapsed = DateTime.Now - StartTime;
                _ellapsedSeconds = ellapsed.TotalSeconds;
                var ellapsedText = ellapsed.ToString(@"mm\:ss");
                Results = $"{Feature} Questions: {items} /  {QuestionsCount} Time: {ellapsedText}";
                Debug.WriteLine("Done Updating Results");
                //Title = $"{Feature}    -    Remaining {QuestionsCount - items}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private void UpdateTitle(Feature feature)
        {
            Title = feature.ToString();
        }


        private bool TestAnswer(int answer)
        {
            return CurrentQuestion.Answer == answer;

        }
        private void NotifyError(int anwswer)
        {
            Task.Run(async () => await _service.NotificationAsync("Error"));
        }

        /// <summary>
        /// Update visible question
        /// </summary>
        private void Populate()
        {
            CurrentQuestion = Questions.Dequeue();
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



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
