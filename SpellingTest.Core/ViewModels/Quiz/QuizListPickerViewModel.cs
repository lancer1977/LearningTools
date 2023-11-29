using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpellingTest.Core.Interfaces;

namespace SpellingTest.Core.ViewModels.Quiz
{
    public class QuizListPickerViewModel : ViewModelAsyncBase
    {
        public ICommand TestCommand { get; }
        public ICommand ActionPickCommand { get; }
        private readonly ISpellingNavigatorService _navigator;
        private readonly IDialogService _displayService;
        private readonly IQuizService _quizService;

        [Reactive] public List<ITopic> Items { get; set; }
        public Guid? SelectedTopic { get; set; }

        public QuizListPickerViewModel(ISpellingNavigatorService navigator, IDialogService displayService, IQuizService quizService)
        {
            _navigator = navigator;
            _displayService = displayService;
            _quizService = quizService;
            ActionPickCommand = ReactiveCommand.CreateFromTask<ITopic>(async x => await ItemSelected(x)).OnException();
            TestCommand = ReactiveCommand.CreateFromTask<ITopic>(async x => await _navigator.ShowQuiz(x)).OnException();
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

