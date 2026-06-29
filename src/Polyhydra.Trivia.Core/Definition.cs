namespace Polyhydra.Trivia.Core;

public sealed record Definition(
    Guid Id,
    Guid TopicId,
    string Term,
    string Text,
    IReadOnlyList<string> Examples,
    ContentStatus Status)
{
    public static Definition Create(Guid topicId, string term, string text, IEnumerable<string>? examples = null)
    {
        if (topicId == Guid.Empty)
        {
            throw new ArgumentException("Topic id is required.", nameof(topicId));
        }

        if (string.IsNullOrWhiteSpace(term))
        {
            throw new ArgumentException("Definition term is required.", nameof(term));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Definition text is required.", nameof(text));
        }

        var normalizedExamples = (examples ?? [])
            .Select(example => example.Trim())
            .Where(example => example.Length > 0)
            .ToArray();

        return new Definition(Guid.NewGuid(), topicId, term.Trim(), text.Trim(), normalizedExamples, ContentStatus.Draft);
    }
}
