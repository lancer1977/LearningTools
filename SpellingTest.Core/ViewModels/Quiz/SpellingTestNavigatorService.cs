using SpellingTest.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace SpellingTest.Core.ViewModels.Quiz;

public class SpellingTestNavigatorService : ISpellingNavigatorService
{
    private readonly INavigatorAsync _navigator;

    public SpellingTestNavigatorService(INavigatorAsync navigator)
    {
        _navigator = navigator;
    }

    public async Task NavigateTo(string path)
    {
        throw new NotImplementedException();
    }

    public async Task ShowQuiz(ITopic topic)
    {
        await _navigator.PushAsync<QuizViewModel>(i => i.LoadAsync(topic.Id.Value));

    }

    public async Task ShowFlashCard(ITopic topic)
    {
        await _navigator.PushAsync<FlashCardViewModel>(i => i.LoadAsync(topic.Id.Value));
    }
}