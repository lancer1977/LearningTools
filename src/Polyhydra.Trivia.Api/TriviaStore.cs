using Microsoft.EntityFrameworkCore;
using Polyhydra.Trivia.Core;
using Polyhydra.Trivia.Infrastructure;

namespace Polyhydra.Trivia.Api;

public interface ITriviaStore
{
    Task<IReadOnlyList<Topic>> ListTopicsAsync(CancellationToken cancellationToken = default);

    Task<Topic?> GetTopicAsync(string slug, CancellationToken cancellationToken = default);

    Task<Topic> AddTopicAsync(CreateTopicRequest request, CancellationToken cancellationToken = default);

    Task<Topic?> UpdateTopicAsync(Guid id, UpdateTopicRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Fact>> ListFactsAsync(Guid topicId, CancellationToken cancellationToken = default);

    Task<Fact?> AddFactAsync(Guid topicId, CreateFactRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Definition>> ListDefinitionsAsync(Guid topicId, CancellationToken cancellationToken = default);

    Task<Definition?> AddDefinitionAsync(Guid topicId, CreateDefinitionRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Question>> ListQuestionsAsync(Guid topicId, CancellationToken cancellationToken = default);

    Task<Question?> AddQuestionAsync(Guid topicId, CreateQuestionRequest request, CancellationToken cancellationToken = default);

    Task<GameSessionSnapshot> StartSessionAsync(CreateGameSessionRequest request, CancellationToken cancellationToken = default);

    Task<GameSessionSnapshot?> GetSessionAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> VoteAsync(Guid sessionId, AnswerRequest request, CancellationToken cancellationToken = default);

    Task<PollResults?> RevealAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<PollResults?> ResultsAsync(Guid sessionId, CancellationToken cancellationToken = default);
}

public sealed class TriviaStore(TriviaDbContext db) : ITriviaStore
{
    public async Task<IReadOnlyList<Topic>> ListTopicsAsync(CancellationToken cancellationToken = default) =>
        (await db.Topics
            .OrderBy(topic => topic.Title)
            .ToArrayAsync(cancellationToken))
        .Select(topic => topic.ToDomain())
        .ToArray();

    public async Task<Topic?> GetTopicAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();
        var topic = await db.Topics.SingleOrDefaultAsync(
            candidate => candidate.Slug == normalizedSlug,
            cancellationToken);
        return topic?.ToDomain();
    }

    public async Task<Topic> AddTopicAsync(CreateTopicRequest request, CancellationToken cancellationToken = default)
    {
        var topic = Topic.Create(request.Title, request.Slug, request.Description, request.Tags);
        db.Topics.Add(new TopicEntity
        {
            Id = topic.Id,
            Title = topic.Title,
            Slug = topic.Slug,
            Description = topic.Description,
            TagsJson = DomainMapping.Serialize(topic.Tags)
        });
        await db.SaveChangesAsync(cancellationToken);
        return topic;
    }

    public async Task<Topic?> UpdateTopicAsync(Guid id, UpdateTopicRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await db.Topics.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var updated = Topic.Create(request.Title, request.Slug, request.Description, request.Tags) with { Id = id };
        entity.Title = updated.Title;
        entity.Slug = updated.Slug;
        entity.Description = updated.Description;
        entity.TagsJson = DomainMapping.Serialize(updated.Tags);
        await db.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<IReadOnlyList<Fact>> ListFactsAsync(Guid topicId, CancellationToken cancellationToken = default) =>
        (await db.Facts
            .Where(fact => fact.TopicId == topicId)
            .OrderBy(fact => fact.Statement)
            .ToArrayAsync(cancellationToken))
        .Select(fact => fact.ToDomain())
        .ToArray();

    public async Task<Fact?> AddFactAsync(Guid topicId, CreateFactRequest request, CancellationToken cancellationToken = default)
    {
        if (!await db.Topics.AnyAsync(topic => topic.Id == topicId, cancellationToken))
        {
            return null;
        }

        var sourceUrl = string.IsNullOrWhiteSpace(request.SourceUrl) ? null : new Uri(request.SourceUrl);
        var fact = Fact.Create(topicId, request.Statement, request.Difficulty, request.Confidence, sourceUrl);
        db.Facts.Add(new FactEntity
        {
            Id = fact.Id,
            TopicId = fact.TopicId,
            Statement = fact.Statement,
            SourceUrl = fact.SourceUrl?.ToString(),
            Difficulty = (int)fact.Difficulty,
            Confidence = fact.Confidence
        });
        await db.SaveChangesAsync(cancellationToken);
        return fact;
    }

    public async Task<IReadOnlyList<Definition>> ListDefinitionsAsync(Guid topicId, CancellationToken cancellationToken = default) =>
        (await db.Definitions
            .Where(definition => definition.TopicId == topicId)
            .OrderBy(definition => definition.Term)
            .ToArrayAsync(cancellationToken))
        .Select(definition => definition.ToDomain())
        .ToArray();

    public async Task<Definition?> AddDefinitionAsync(Guid topicId, CreateDefinitionRequest request, CancellationToken cancellationToken = default)
    {
        if (!await db.Topics.AnyAsync(topic => topic.Id == topicId, cancellationToken))
        {
            return null;
        }

        var definition = Definition.Create(topicId, request.Term, request.Text, request.Examples);
        db.Definitions.Add(new DefinitionEntity
        {
            Id = definition.Id,
            TopicId = definition.TopicId,
            Term = definition.Term,
            Text = definition.Text,
            ExamplesJson = DomainMapping.Serialize(definition.Examples)
        });
        await db.SaveChangesAsync(cancellationToken);
        return definition;
    }

    public async Task<IReadOnlyList<Question>> ListQuestionsAsync(Guid topicId, CancellationToken cancellationToken = default) =>
        (await db.Questions
            .Where(question => question.TopicId == topicId)
            .OrderBy(question => question.Prompt)
            .ToArrayAsync(cancellationToken))
        .Select(question => question.ToDomain())
        .ToArray();

    public async Task<Question?> AddQuestionAsync(Guid topicId, CreateQuestionRequest request, CancellationToken cancellationToken = default)
    {
        if (!await db.Topics.AnyAsync(topic => topic.Id == topicId, cancellationToken))
        {
            return null;
        }

        var choices = request.Choices.Select(choice => AnswerChoice.Create(choice.Id, choice.Text));
        var question = Question.Create(
            topicId,
            request.Prompt,
            choices,
            request.CorrectAnswerId,
            request.Explanation,
            request.Difficulty,
            request.SourceFactId);
        db.Questions.Add(new QuestionEntity
        {
            Id = question.Id,
            TopicId = question.TopicId,
            Prompt = question.Prompt,
            ChoicesJson = DomainMapping.Serialize(question.Choices),
            CorrectAnswerId = question.CorrectAnswerId,
            Explanation = question.Explanation,
            Difficulty = (int)question.Difficulty,
            SourceFactId = question.SourceFactId,
            Status = (int)question.Status
        });
        await db.SaveChangesAsync(cancellationToken);
        return question;
    }

    public async Task<GameSessionSnapshot> StartSessionAsync(CreateGameSessionRequest request, CancellationToken cancellationToken = default)
    {
        var topicIds = request.TopicIds.Length == 0
            ? await db.Topics.Select(topic => topic.Id).ToArrayAsync(cancellationToken)
            : request.TopicIds;
        var question = await db.Questions.FirstAsync(candidate => topicIds.Contains(candidate.TopicId), cancellationToken);
        var session = GameSession.Start(request.Mode, topicIds, question.Id);
        var entity = new GameSessionEntity
        {
            Id = session.Id,
            Mode = (int)session.Mode,
            TopicIdsJson = DomainMapping.Serialize(session.TopicIds),
            CurrentQuestionId = session.CurrentQuestionId,
            IsVotingClosed = session.IsVotingClosed,
            IsRevealed = session.IsRevealed
        };
        db.GameSessions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.ToSnapshot([]);
    }

    public async Task<GameSessionSnapshot?> GetSessionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await db.GameSessions.FindAsync([id], cancellationToken);
        if (session is null)
        {
            return null;
        }

        var votes = await db.AnswerVotes
            .Where(vote => vote.GameSessionId == id && vote.QuestionId == session.CurrentQuestionId)
            .ToArrayAsync(cancellationToken);
        return session.ToSnapshot(votes);
    }

    public async Task<bool> VoteAsync(Guid sessionId, AnswerRequest request, CancellationToken cancellationToken = default)
    {
        var session = await db.GameSessions.FindAsync([sessionId], cancellationToken);
        if (session is null)
        {
            return false;
        }

        if (session.IsVotingClosed)
        {
            throw new InvalidOperationException("Voting is closed.");
        }

        var voterId = RequireTrimmed(request.VoterId, nameof(request.VoterId));
        var answerId = RequireTrimmed(request.AnswerId, nameof(request.AnswerId)).ToUpperInvariant();
        var existing = await db.AnswerVotes.SingleOrDefaultAsync(
            vote => vote.GameSessionId == sessionId
                && vote.QuestionId == session.CurrentQuestionId
                && vote.VoterId.ToLower() == voterId.ToLower(),
            cancellationToken);
        if (existing is null)
        {
            db.AnswerVotes.Add(new AnswerVoteEntity
            {
                Id = Guid.NewGuid(),
                GameSessionId = sessionId,
                QuestionId = session.CurrentQuestionId,
                VoterId = voterId,
                AnswerId = answerId
            });
        }
        else
        {
            existing.VoterId = voterId;
            existing.AnswerId = answerId;
        }

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PollResults?> RevealAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await db.GameSessions.FindAsync([sessionId], cancellationToken);
        if (session is null)
        {
            return null;
        }

        session.IsVotingClosed = true;
        session.IsRevealed = true;
        await db.SaveChangesAsync(cancellationToken);
        return await ResultsAsync(sessionId, cancellationToken);
    }

    public async Task<PollResults?> ResultsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSessionAsync(sessionId, cancellationToken);
        if (snapshot is null)
        {
            return null;
        }

        var question = await db.Questions.FindAsync([snapshot.CurrentQuestionId], cancellationToken);
        if (question is null)
        {
            return null;
        }

        return CalculateResults(snapshot, question.ToDomain());
    }

    private static PollResults CalculateResults(GameSessionSnapshot session, Question question)
    {
        var total = session.Votes.Count;
        var choices = question.Choices
            .Select(choice =>
            {
                var count = session.Votes.Count(vote => string.Equals(vote.AnswerId, choice.Id, StringComparison.OrdinalIgnoreCase));
                var percentage = total == 0 ? 0 : Math.Round(count * 100m / total, 2);
                return new PollChoiceResult(choice.Id, count, percentage);
            })
            .ToArray();
        var majority = choices
            .OrderByDescending(choice => choice.Count)
            .ThenBy(choice => choice.AnswerId, StringComparer.Ordinal)
            .FirstOrDefault(choice => choice.Count > 0)
            ?.AnswerId;

        return new PollResults(
            question.Id,
            session.IsRevealed,
            session.IsRevealed ? question.CorrectAnswerId : null,
            session.IsVotingClosed ? majority : null,
            choices);
    }

    private static string RequireTrimmed(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }

        return value.Trim();
    }
}

public sealed record CreateTopicRequest(string Title, string Slug, string Description, string[] Tags);

public sealed record UpdateTopicRequest(string Title, string Slug, string Description, string[] Tags);

public sealed record CreateFactRequest(string Statement, string? SourceUrl, Difficulty Difficulty, decimal Confidence);

public sealed record CreateDefinitionRequest(string Term, string Text, string[] Examples);

public sealed record CreateAnswerChoiceRequest(string Id, string Text);

public sealed record CreateQuestionRequest(
    string Prompt,
    CreateAnswerChoiceRequest[] Choices,
    string CorrectAnswerId,
    string Explanation,
    Difficulty Difficulty,
    Guid? SourceFactId);

public sealed record CreateGameSessionRequest(GameMode Mode, Guid[] TopicIds);

public sealed record AnswerRequest(string VoterId, string AnswerId);
