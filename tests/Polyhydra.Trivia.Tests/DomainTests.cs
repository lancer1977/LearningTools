using Polyhydra.Trivia.Core;

namespace Polyhydra.Trivia.Tests;

public sealed class DomainTests
{
    [Fact]
    public void TopicCreateNormalizesSlugAndTags()
    {
        var topic = Topic.Create("  Modern .NET  ", "Modern-Dotnet", "Platform trivia", [" DotNet ", "dotnet", "web"]);

        Assert.Equal("Modern .NET", topic.Title);
        Assert.Equal("modern-dotnet", topic.Slug);
        Assert.Equal(["dotnet", "web"], topic.Tags);
    }

    [Fact]
    public void QuestionRequiresCorrectAnswerToMatchAChoice()
    {
        var topic = Topic.Create("Modern .NET", "modern-dotnet", "Platform trivia");

        var exception = Assert.Throws<ArgumentException>(() => Question.Create(
            topic.Id,
            "Which answer is valid?",
            [AnswerChoice.Create("A", "One"), AnswerChoice.Create("B", "Two")],
            "C",
            "The correct answer must exist.",
            Difficulty.Easy));

        Assert.Contains("Correct answer id", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AiGeneratedQuestionCannotPublishWithoutHumanReview()
    {
        var topic = Topic.Create("Modern .NET", "modern-dotnet", "Platform trivia");
        var question = Question.Create(
            topic.Id,
            "Which status must happen before publish?",
            [AnswerChoice.Create("A", "AI generated"), AnswerChoice.Create("B", "Human reviewed")],
            "B",
            "Generated content must be reviewed before publishing.",
            Difficulty.Medium,
            status: ContentStatus.AiGenerated);

        Assert.Throws<InvalidOperationException>(() => question.Publish());

        var published = question.MarkHumanReviewed().Publish();
        Assert.Equal(ContentStatus.Published, published.Status);
    }

    [Fact]
    public void GameSessionReplacesDuplicateVotesAndCalculatesPollResults()
    {
        var topic = Topic.Create("Modern .NET", "modern-dotnet", "Platform trivia");
        var question = Question.Create(
            topic.Id,
            "Which endpoint style is compact?",
            [AnswerChoice.Create("A", "Minimal API"), AnswerChoice.Create("B", "MVC controller")],
            "A",
            "Minimal APIs are compact endpoint definitions.",
            Difficulty.Easy);
        var session = GameSession.Start(GameMode.ChatPoll, [topic.Id], question.Id);

        session.Vote("viewer-1", "B");
        session.Vote("viewer-1", "A");
        session.Vote("viewer-2", "A");
        session.CloseVoting();

        var closedResults = session.GetResults(question);

        Assert.False(closedResults.IsRevealed);
        Assert.Null(closedResults.CorrectAnswerId);
        Assert.Equal("A", closedResults.MajorityAnswerId);
        Assert.Equal(2, closedResults.Choices.Single(choice => choice.AnswerId == "A").Count);
        Assert.Equal(100m, closedResults.Choices.Single(choice => choice.AnswerId == "A").Percentage);

        session.Reveal();
        var revealedResults = session.GetResults(question);

        Assert.True(revealedResults.IsRevealed);
        Assert.Equal("A", revealedResults.CorrectAnswerId);
    }
}
