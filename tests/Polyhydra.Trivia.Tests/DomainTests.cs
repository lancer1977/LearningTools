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
        var metadata = new GeneratedQuestionMetadata(
            "test-provider",
            "test-model",
            "facts-to-question-v1",
            DateTimeOffset.UtcNow,
            "One fact about content review.");
        var question = Question.Create(
            topic.Id,
            "Which status must happen before publish?",
            [AnswerChoice.Create("A", "AI generated"), AnswerChoice.Create("B", "Human reviewed")],
            "B",
            "Generated content must be reviewed before publishing.",
            Difficulty.Medium,
            status: ContentStatus.AiGenerated,
            generatedMetadata: metadata);

        Assert.Throws<InvalidOperationException>(() => question.Publish());

        var published = question.MarkHumanReviewed().Publish();
        Assert.Equal(ContentStatus.Published, published.Status);
    }

    [Fact]
    public async Task AiTriviaWorkflowStoresGeneratedContentForReview()
    {
        var topic = Topic.Create("Modern .NET", "modern-dotnet", "Platform trivia");
        var fact = Fact.Create(
            topic.Id,
            ".NET supports web applications through ASP.NET Core.",
            Difficulty.Easy,
            0.95m,
            new Uri("https://dotnet.microsoft.com/apps/aspnet"));
        var metadata = new GeneratedQuestionMetadata(
            "fixture-provider",
            "fixture-model",
            "facts-to-question-v1",
            DateTimeOffset.UtcNow,
            "Generated from the ASP.NET Core fact.");
        var workflow = new AiTriviaContentWorkflow(new FixedTriviaQuestionGenerator(metadata));

        var question = await workflow.GenerateQuestionFromFactsAsync(
            topic,
            [fact],
            Difficulty.Easy,
            "Create one beginner-friendly multiple-choice question.");

        Assert.Equal(ContentStatus.AiGenerated, question.Status);
        Assert.Equal(metadata, question.GeneratedMetadata);
        Assert.Equal(fact.Id, question.SourceFactId);
        Assert.Contains("ASP.NET Core", question.Explanation, StringComparison.Ordinal);
        Assert.Throws<InvalidOperationException>(() => question.Publish());
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

internal sealed class FixedTriviaQuestionGenerator(GeneratedQuestionMetadata metadata) : ITriviaQuestionGenerator
{
    public Task<GeneratedTriviaQuestionDraft> GenerateQuestionAsync(
        AiTriviaGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        Assert.Equal("Modern .NET", request.TopicTitle);
        Assert.Single(request.Facts);

        return Task.FromResult(new GeneratedTriviaQuestionDraft(
            "Which .NET technology is used for web applications?",
            [AnswerChoice.Create("A", "ASP.NET Core"), AnswerChoice.Create("B", "WinForms")],
            "A",
            "ASP.NET Core is the .NET web application framework.",
            request.Difficulty,
            metadata));
    }
}
