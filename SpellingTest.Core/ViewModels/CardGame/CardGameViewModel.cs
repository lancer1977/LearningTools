using System.Reactive;

namespace SpellingTest.Core.ViewModels.CardGame
{
    //0. Pick Player / Leader, leader will share the room #
    //1. Is initialized with a collection of Image/Name objects
    //2. The list is shuffled
    //3. 3 items are pulled into active
    //4. 1 of the 3 is picked as the real selection
    //5. the 3 image options and the real choice are sent to the game master.  
    //6. 

    /// <summary>
    /// Presents an Image and asks the user to pick the name
    /// </summary>
    public class CardGameViewModel : ViewModelAsyncBase, ILoadAsync<Guid>
    {
        private readonly IQuizService _quizService;
        private readonly ITextToSpeech _textToSpeech;
        private Guid _currentGameId;
        public CardGameViewModel(IQuizService quizService, ITextToSpeech textToSpeech)
        {
            _quizService = quizService;
            _textToSpeech = textToSpeech;
            Message = "Pick a card";
            ResetCommand = ReactiveCommand.CreateFromTask(async () => await ResetGame());
            SelectCard = ReactiveCommand.CreateFromTask<string, Unit>(async (x) =>
             {
                 var response = Game.PickCard(x);
                 Message = response.Message;

                 if (response.GameOver)
                 {
                     await ResetGame();
                 }
                 return Unit.Default;
             });
            this.WhenAnyValue(x => x.ShowImage, x => x.ShowName, (image, name) => (image, name))
                .Subscribe((x) => Game?.SetVisibility(x.name, x.image));

        }

        private async Task ResetGame()
        {
            Game = new CardGame(_textToSpeech);
            var quiz = await _quizService.GetQuiz(_currentGameId);
            var cards = quiz.Definitions.Select(x => new Card(x));
            Game.Load(cards);
            Game.Initialize();
        }

        public override async Task StartAsync()
        {

            await ResetGame();
            await base.StartAsync();
        }

        [Reactive] public CardGame Game { get; set; }
        [Reactive] public bool ShowName { get; set; }
        [Reactive] public bool ShowImage { get; set; }
        public ICommand SelectCard { get; }
        public ICommand ResetCommand { get; }

        [Reactive] public string Message { get; set; }

        public async Task LoadAsync(Guid value)
        {
            _currentGameId = value;
            await Task.Delay(1);
        }
    }
}
