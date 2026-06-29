namespace Polyhydra.Trivia.Core;

public sealed record Topic(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    IReadOnlyList<string> Tags)
{
    public static Topic Create(string title, string slug, string description, IEnumerable<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Topic title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Topic slug is required.", nameof(slug));
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();
        if (normalizedSlug.Any(c => !(char.IsLetterOrDigit(c) || c == '-')))
        {
            throw new ArgumentException("Topic slug can contain only lowercase letters, numbers, and hyphens.", nameof(slug));
        }

        var normalizedTags = (tags ?? [])
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Where(tag => tag.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return new Topic(Guid.NewGuid(), title.Trim(), normalizedSlug, description.Trim(), normalizedTags);
    }
}
