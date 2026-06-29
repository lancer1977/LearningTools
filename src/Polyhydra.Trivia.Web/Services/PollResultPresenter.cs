using Polyhydra.Trivia.Core;

namespace Polyhydra.Trivia.Web.Services;

public sealed class PollResultPresenter
{
    public PollResultsView Create(Question question, PollResults results, string? streamerAnswerId = null)
    {
        if (question.Id != results.QuestionId)
        {
            throw new ArgumentException("Poll results must match the displayed question.", nameof(results));
        }

        var choices = question.Choices
            .Select(choice =>
            {
                var result = results.Choices.SingleOrDefault(candidate => string.Equals(candidate.AnswerId, choice.Id, StringComparison.OrdinalIgnoreCase))
                    ?? new PollChoiceResult(choice.Id, 0, 0);
                var badges = new List<string>();

                if (results.IsRevealed && string.Equals(results.CorrectAnswerId, choice.Id, StringComparison.OrdinalIgnoreCase))
                {
                    badges.Add("Correct");
                }

                if (string.Equals(results.MajorityAnswerId, choice.Id, StringComparison.OrdinalIgnoreCase))
                {
                    badges.Add("Chat majority");
                }

                if (!string.IsNullOrWhiteSpace(streamerAnswerId) && string.Equals(streamerAnswerId, choice.Id, StringComparison.OrdinalIgnoreCase))
                {
                    badges.Add("Streamer");
                }

                var cssClass = string.Join(
                    ' ',
                    new[]
                    {
                        badges.Contains("Correct", StringComparer.Ordinal) ? "is-correct" : null,
                        badges.Contains("Chat majority", StringComparer.Ordinal) ? "is-majority" : null,
                        badges.Contains("Streamer", StringComparer.Ordinal) ? "is-streamer" : null
                    }.Where(value => value is not null));

                return new PollChoiceView(
                    choice.Id,
                    choice.Text,
                    result.Count,
                    result.Percentage,
                    badges,
                    cssClass);
            })
            .ToArray();

        return new PollResultsView(results.IsRevealed, choices);
    }
}

public sealed record PollResultsView(bool IsRevealed, IReadOnlyList<PollChoiceView> Choices);

public sealed record PollChoiceView(
    string AnswerId,
    string Text,
    int Count,
    decimal Percentage,
    IReadOnlyList<string> Badges,
    string CssClass);
