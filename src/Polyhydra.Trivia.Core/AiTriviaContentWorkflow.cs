namespace Polyhydra.Trivia.Core;

public sealed record AiTriviaGenerationRequest(
    Guid TopicId,
    string TopicTitle,
    IReadOnlyList<Fact> Facts,
    Difficulty Difficulty,
    string PromptInstructions);

public sealed record GeneratedQuestionMetadata(
    string Provider,
    string Model,
    string PromptVersion,
    DateTimeOffset GeneratedAt,
    string SourceSummary)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Provider))
        {
            throw new InvalidOperationException("Generated content provider is required.");
        }

        if (string.IsNullOrWhiteSpace(Model))
        {
            throw new InvalidOperationException("Generated content model is required.");
        }

        if (string.IsNullOrWhiteSpace(PromptVersion))
        {
            throw new InvalidOperationException("Generated content prompt version is required.");
        }

        if (GeneratedAt == default)
        {
            throw new InvalidOperationException("Generated content timestamp is required.");
        }

        if (string.IsNullOrWhiteSpace(SourceSummary))
        {
            throw new InvalidOperationException("Generated content source summary is required.");
        }
    }
}

public sealed record GeneratedTriviaQuestionDraft(
    string Prompt,
    IReadOnlyList<AnswerChoice> Choices,
    string CorrectAnswerId,
    string Explanation,
    Difficulty Difficulty,
    GeneratedQuestionMetadata Metadata);

public interface ITriviaQuestionGenerator
{
    Task<GeneratedTriviaQuestionDraft> GenerateQuestionAsync(
        AiTriviaGenerationRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class AiTriviaContentWorkflow(ITriviaQuestionGenerator generator)
{
    public async Task<Question> GenerateQuestionFromFactsAsync(
        Topic topic,
        IReadOnlyList<Fact> facts,
        Difficulty difficulty,
        string promptInstructions,
        CancellationToken cancellationToken = default)
    {
        ValidateTopicFacts(topic, facts);

        var request = new AiTriviaGenerationRequest(
            topic.Id,
            topic.Title,
            facts,
            difficulty,
            promptInstructions.Trim());

        var generated = await generator.GenerateQuestionAsync(request, cancellationToken);
        generated.Metadata.Validate();

        return Question.Create(
            topic.Id,
            generated.Prompt,
            generated.Choices,
            generated.CorrectAnswerId,
            generated.Explanation,
            generated.Difficulty,
            facts.FirstOrDefault()?.Id,
            ContentStatus.AiGenerated,
            generated.Metadata);
    }

    private static void ValidateTopicFacts(Topic topic, IReadOnlyList<Fact> facts)
    {
        if (facts.Any(fact => fact.TopicId != topic.Id))
        {
            throw new ArgumentException("Generated trivia facts must belong to the target topic.", nameof(facts));
        }
    }
}
