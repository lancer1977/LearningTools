namespace Polyhydra.Trivia.Core;

public sealed record Fact(
    Guid Id,
    Guid TopicId,
    string Statement,
    Uri? SourceUrl,
    Difficulty Difficulty,
    decimal Confidence,
    ContentStatus Status)
{
    public static Fact Create(Guid topicId, string statement, Difficulty difficulty, decimal confidence, Uri? sourceUrl = null)
    {
        if (topicId == Guid.Empty)
        {
            throw new ArgumentException("Topic id is required.", nameof(topicId));
        }

        if (string.IsNullOrWhiteSpace(statement))
        {
            throw new ArgumentException("Fact statement is required.", nameof(statement));
        }

        if (confidence is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(confidence), "Confidence must be between 0 and 1.");
        }

        return new Fact(Guid.NewGuid(), topicId, statement.Trim(), sourceUrl, difficulty, confidence, ContentStatus.Draft);
    }
}
