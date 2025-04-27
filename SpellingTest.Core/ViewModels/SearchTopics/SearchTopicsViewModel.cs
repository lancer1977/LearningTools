namespace SpellingTest.Core.ViewModels.SearchTopics
{
    public class SearchTopicsViewModel : ViewModelAsyncBase
    {
        public ICommand TestCommand { get; }
        public ICommand SearchCommand { get; }

        private readonly ISpellingNavigatorService _navigator;
        private readonly IDialogService _displayService;
        private readonly IQuizService _quizService;

        [Reactive] public List<ITopic> Items { get; set; }
        public Guid? SelectedTopic { get; set; }
        private TopicSearch Query => new TopicSearch
        {
            Description = Description,
            IncludeFavorites = IncludeFavorites,
            IncludeNonFavorites = IncludeNonFavorites,
            MaxCount = MaxCount,
            Name = Name
        };

        [Reactive] public string Name { get; set; }

        [Reactive] public int? MaxCount { get; set; }

        [Reactive] public bool? IncludeNonFavorites { get; set; }

        [Reactive] public bool? IncludeFavorites { get; set; }

        [Reactive] public string Description { get; set; }

        public SearchTopicsViewModel(ISpellingNavigatorService navigator, IDialogService displayService, IQuizService quizService)
        {
            _navigator = navigator;
            _displayService = displayService;
            _quizService = quizService;
            TestCommand = ReactiveCommand.CreateFromTask<ITopic>(async x => await _navigator.ShowQuiz(x)).OnException();
            SearchCommand = ReactiveCommand.CreateFromTask(async () => await quizService.SearchTopic(Query));
            this.WhenAnyValue(
                x => x.Description,
                x => x.MaxCount,
                x => x.IncludeFavorites,
            x => x.IncludeNonFavorites,
                (des, count, favs, nofavs) => new TopicSearch
                {
                    Description = des,
                    MaxCount = count,
                    IncludeFavorites = favs,
                    IncludeNonFavorites = nofavs,

                })
                .Select(async x =>
            {

                Items?.Clear();
                var response = await quizService.SearchTopic(x);
                Items = response.ToList();

            }).Subscribe();

        }


        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            await RefreshItems();
        }



        private async Task RefreshItems()
        {
            try
            {
                var item = await _quizService.GetTopics();
                Items = item.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }



        public async Task ItemSelected(ITopic model)
        {
            var selection = await _displayService.InputBoxAsync("Action", new string[] { "Test", "Flash Card", });
            {
                switch (selection.Result)
                {
                    case "Test":
                        TestCommand.Execute(model);

                        break;
                    case "Flash Card":
                        await _navigator.ShowFlashCard(model);
                        break;

                    case "Delete":

                        if ((await _displayService.GetBooleanAsync("Are you sure you wish to delete this list?", "Delete Warning", "Yes", "No")).Result)
                        {
                            await _displayService.NotificationAsync("NOT IMPLEMENTED");
                        }

                        break;
                }
            }

        }

    }


}

