namespace Polyhydra.Trivia.Core;

public sealed record AnswerChoice(string Id, string Text)
{
    public static AnswerChoice Create(string id, string text)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Answer id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Answer text is required.", nameof(text));
        }

        return new AnswerChoice(id.Trim().ToUpperInvariant(), text.Trim());
    }
}

public sealed record Question(
    Guid Id,
    Guid TopicId,
    string Prompt,
    IReadOnlyList<AnswerChoice> Choices,
    string CorrectAnswerId,
    string Explanation,
    Difficulty Difficulty,
    Guid? SourceFactId,
    ContentStatus Status)
{
    public static Question Create(
        Guid topicId,
        string prompt,
        IEnumerable<AnswerChoice> choices,
        string correctAnswerId,
        string explanation,
        Difficulty difficulty,
        Guid? sourceFactId = null,
        ContentStatus status = ContentStatus.Draft)
    {
        if (topicId == Guid.Empty)
        {
            throw new ArgumentException("Topic id is required.", nameof(topicId));
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Question prompt is required.", nameof(prompt));
        }

        var normalizedChoices = choices.ToArray();
        if (normalizedChoices.Length < 2)
        {
            throw new ArgumentException("At least two answer choices are required.", nameof(choices));
        }

        if (normalizedChoices.Select(choice => choice.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count() != normalizedChoices.Length)
        {
            throw new ArgumentException("Answer choice ids must be unique.", nameof(choices));
        }

        var normalizedCorrectAnswerId = correctAnswerId.Trim().ToUpperInvariant();
        if (!normalizedChoices.Any(choice => string.Equals(choice.Id, normalizedCorrectAnswerId, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Correct answer id must match one of the choices.", nameof(correctAnswerId));
        }

        if (string.IsNullOrWhiteSpace(explanation))
        {
            throw new ArgumentException("Question explanation is required.", nameof(explanation));
        }

        if (status == ContentStatus.Published)
        {
            throw new ArgumentException("Questions must be human reviewed before publishing.", nameof(status));
        }

        return new Question(
            Guid.NewGuid(),
            topicId,
            prompt.Trim(),
            normalizedChoices,
            normalizedCorrectAnswerId,
            explanation.Trim(),
            difficulty,
            sourceFactId,
            status);
    }

    public Question MarkHumanReviewed() => this with { Status = ContentStatus.HumanReviewed };

    public Question Publish()
    {
        if (Status != ContentStatus.HumanReviewed)
        {
            throw new InvalidOperationException("Only human-reviewed questions can be published.");
        }

        return this with { Status = ContentStatus.Published };
    }
}
