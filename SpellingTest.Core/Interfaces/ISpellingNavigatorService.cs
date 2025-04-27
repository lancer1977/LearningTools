namespace SpellingTest.Core.Interfaces;

public interface ISpellingNavigatorService
{
    Task NavigateTo(string path);
    Task ShowQuiz(ITopic topic);
    Task ShowFlashCard(ITopic topic);
}