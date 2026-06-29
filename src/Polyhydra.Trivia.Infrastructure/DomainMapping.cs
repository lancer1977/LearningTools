using System.Text.Json;
using Polyhydra.Trivia.Core;

namespace Polyhydra.Trivia.Infrastructure;

public static class DomainMapping
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Topic ToDomain(this TopicEntity entity) =>
        Topic.Create(entity.Title, entity.Slug, entity.Description, Deserialize<string>(entity.TagsJson)) with { Id = entity.Id };

    public static Fact ToDomain(this FactEntity entity) =>
        Fact.Create(
            entity.TopicId,
            entity.Statement,
            (Difficulty)entity.Difficulty,
            entity.Confidence,
            string.IsNullOrWhiteSpace(entity.SourceUrl) ? null : new Uri(entity.SourceUrl)) with { Id = entity.Id };

    public static Definition ToDomain(this DefinitionEntity entity) =>
        Definition.Create(
            entity.TopicId,
            entity.Term,
            entity.Text,
            Deserialize<string>(entity.ExamplesJson)) with { Id = entity.Id };

    public static Question ToDomain(this QuestionEntity entity)
    {
        var question = Question.Create(
            entity.TopicId,
            entity.Prompt,
            Deserialize<AnswerChoice>(entity.ChoicesJson),
            entity.CorrectAnswerId,
            entity.Explanation,
            (Difficulty)entity.Difficulty,
            entity.SourceFactId) with { Id = entity.Id };

        return (ContentStatus)entity.Status switch
        {
            ContentStatus.Draft => question,
            ContentStatus.HumanReviewed => question.MarkHumanReviewed(),
            ContentStatus.Published => question.MarkHumanReviewed().Publish(),
            _ => question
        };
    }

    public static GameSessionSnapshot ToSnapshot(
        this GameSessionEntity entity,
        IEnumerable<AnswerVoteEntity> votes) =>
        new(
            entity.Id,
            (GameMode)entity.Mode,
            Deserialize<Guid>(entity.TopicIdsJson),
            entity.CurrentQuestionId,
            entity.IsVotingClosed,
            entity.IsRevealed,
            votes.Select(vote => new AnswerVote(vote.VoterId, vote.AnswerId)).ToArray());

    public static string Serialize<T>(IEnumerable<T> values) => JsonSerializer.Serialize(values, JsonOptions);

    private static IReadOnlyList<T> Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T[]>(json, JsonOptions) ?? Array.Empty<T>();
}

public sealed record GameSessionSnapshot(
    Guid Id,
    GameMode Mode,
    IReadOnlyList<Guid> TopicIds,
    Guid CurrentQuestionId,
    bool IsVotingClosed,
    bool IsRevealed,
    IReadOnlyList<AnswerVote> Votes);
