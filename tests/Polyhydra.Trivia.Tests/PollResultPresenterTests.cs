using Polyhydra.Trivia.Core;
using Polyhydra.Trivia.Web.Services;

namespace Polyhydra.Trivia.Tests;

public sealed class PollResultPresenterTests
{
    [Fact]
    public void PresenterHidesCorrectBadgeUntilResultsAreRevealed()
    {
        var question = CreateQuestion();
        var results = new PollResults(
            question.Id,
            false,
            null,
            "B",
            [
                new PollChoiceResult("A", 1, 25),
                new PollChoiceResult("B", 3, 75)
            ]);

        var view = PollResultPresenter.Create(question, results);

        Assert.False(view.IsRevealed);
        Assert.DoesNotContain(view.Choices.Single(choice => choice.AnswerId == "A").Badges, badge => badge == "Correct");
        Assert.Contains("Chat majority", view.Choices.Single(choice => choice.AnswerId == "B").Badges);
    }

    [Fact]
    public void PresenterHighlightsCorrectMajorityAndStreamerChoicesAfterReveal()
    {
        var question = CreateQuestion();
        var results = new PollResults(
            question.Id,
            true,
            "A",
            "A",
            [
                new PollChoiceResult("A", 4, 80),
                new PollChoiceResult("B", 1, 20)
            ]);

        var view = PollResultPresenter.Create(question, results, "B");

        var correct = view.Choices.Single(choice => choice.AnswerId == "A");
        Assert.Contains("Correct", correct.Badges);
        Assert.Contains("Chat majority", correct.Badges);
        Assert.Contains("is-correct", correct.CssClass);

        var streamer = view.Choices.Single(choice => choice.AnswerId == "B");
        Assert.Contains("Streamer", streamer.Badges);
        Assert.Contains("is-streamer", streamer.CssClass);
    }

    private static Question CreateQuestion()
    {
        var topic = Topic.Create("Modern .NET", "modern-dotnet", "Platform trivia");
        return Question.Create(
            topic.Id,
            "Which endpoint style is compact?",
            [AnswerChoice.Create("A", "Minimal API"), AnswerChoice.Create("B", "MVC controller")],
            "A",
            "Minimal APIs are compact endpoint definitions.",
            Difficulty.Easy);
    }
}
