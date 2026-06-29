namespace Polyhydra.Trivia.Core;

public sealed record AnswerVote(string VoterId, string AnswerId);

public sealed record PollChoiceResult(string AnswerId, int Count, decimal Percentage);

public sealed record PollResults(
    Guid QuestionId,
    bool IsRevealed,
    string? CorrectAnswerId,
    string? MajorityAnswerId,
    IReadOnlyList<PollChoiceResult> Choices);

public sealed class GameSession
{
    private readonly Dictionary<Guid, List<AnswerVote>> votesByQuestion = [];

    public GameSession(Guid id, GameMode mode, IReadOnlyList<Guid> topicIds, Guid currentQuestionId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Session id is required.", nameof(id));
        }

        if (topicIds.Count == 0)
        {
            throw new ArgumentException("At least one topic is required.", nameof(topicIds));
        }

        if (currentQuestionId == Guid.Empty)
        {
            throw new ArgumentException("Current question id is required.", nameof(currentQuestionId));
        }

        Id = id;
        Mode = mode;
        TopicIds = topicIds;
        CurrentQuestionId = currentQuestionId;
    }

    public Guid Id { get; }

    public GameMode Mode { get; }

    public IReadOnlyList<Guid> TopicIds { get; }

    public Guid CurrentQuestionId { get; private set; }

    public bool IsVotingClosed { get; private set; }

    public bool IsRevealed { get; private set; }

    public static GameSession Start(GameMode mode, IEnumerable<Guid> topicIds, Guid currentQuestionId) =>
        new(Guid.NewGuid(), mode, topicIds.ToArray(), currentQuestionId);

    public void Vote(string voterId, string answerId)
    {
        if (IsVotingClosed)
        {
            throw new InvalidOperationException("Voting is closed.");
        }

        if (string.IsNullOrWhiteSpace(voterId))
        {
            throw new ArgumentException("Voter id is required.", nameof(voterId));
        }

        if (string.IsNullOrWhiteSpace(answerId))
        {
            throw new ArgumentException("Answer id is required.", nameof(answerId));
        }

        var normalizedAnswerId = answerId.Trim().ToUpperInvariant();
        var votes = votesByQuestion.GetValueOrDefault(CurrentQuestionId);
        if (votes is null)
        {
            votes = [];
            votesByQuestion[CurrentQuestionId] = votes;
        }

        var existing = votes.FindIndex(vote => string.Equals(vote.VoterId, voterId.Trim(), StringComparison.OrdinalIgnoreCase));
        var vote = new AnswerVote(voterId.Trim(), normalizedAnswerId);
        if (existing >= 0)
        {
            votes[existing] = vote;
            return;
        }

        votes.Add(vote);
    }

    public void CloseVoting() => IsVotingClosed = true;

    public void Reveal()
    {
        IsVotingClosed = true;
        IsRevealed = true;
    }

    public PollResults GetResults(Question question)
    {
        if (question.Id != CurrentQuestionId)
        {
            throw new ArgumentException("Question does not match the current session question.", nameof(question));
        }

        var votes = votesByQuestion.GetValueOrDefault(CurrentQuestionId) ?? [];
        var total = votes.Count;
        var results = question.Choices
            .Select(choice =>
            {
                var count = votes.Count(vote => string.Equals(vote.AnswerId, choice.Id, StringComparison.OrdinalIgnoreCase));
                var percentage = total == 0 ? 0 : Math.Round(count * 100m / total, 2);
                return new PollChoiceResult(choice.Id, count, percentage);
            })
            .ToArray();

        var majority = results
            .OrderByDescending(result => result.Count)
            .ThenBy(result => result.AnswerId, StringComparer.Ordinal)
            .FirstOrDefault(result => result.Count > 0)
            ?.AnswerId;

        return new PollResults(
            question.Id,
            IsRevealed,
            IsRevealed ? question.CorrectAnswerId : null,
            IsVotingClosed ? majority : null,
            results);
    }
}
