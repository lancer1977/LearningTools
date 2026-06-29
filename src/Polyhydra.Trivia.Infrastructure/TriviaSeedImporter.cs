using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Polyhydra.Trivia.Core;

namespace Polyhydra.Trivia.Infrastructure;

public sealed class TriviaSeedImporter(TriviaDbContext db)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task ImportDirectoryAsync(string seedDirectory, CancellationToken cancellationToken = default)
    {
        foreach (var file in Directory.EnumerateFiles(seedDirectory, "*.json").Order(StringComparer.Ordinal))
        {
            await using var stream = File.OpenRead(file);
            var document = await JsonSerializer.DeserializeAsync<TriviaSeedDocument>(stream, JsonOptions, cancellationToken)
                ?? throw new InvalidOperationException($"Seed file '{file}' could not be parsed.");

            await ImportAsync(document, cancellationToken);
        }
    }

    public async Task ImportAsync(TriviaSeedDocument document, CancellationToken cancellationToken = default)
    {
        foreach (var seedTopic in document.Topics)
        {
            var topicEntity = await UpsertTopicAsync(seedTopic, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);

            var factIdsByKey = await UpsertFactsAsync(topicEntity.Id, seedTopic.Facts, cancellationToken);
            await UpsertDefinitionsAsync(topicEntity.Id, seedTopic.Definitions, cancellationToken);
            await UpsertQuestionsAsync(topicEntity.Id, seedTopic.Questions, factIdsByKey, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<TopicEntity> UpsertTopicAsync(TriviaSeedTopic seedTopic, CancellationToken cancellationToken)
    {
        var topic = Topic.Create(seedTopic.Title, seedTopic.Slug, seedTopic.Description, seedTopic.Tags);
        var topicEntity = await db.Topics.SingleOrDefaultAsync(candidate => candidate.Slug == topic.Slug, cancellationToken);
        if (topicEntity is null)
        {
            topicEntity = new TopicEntity { Id = topic.Id };
            db.Topics.Add(topicEntity);
        }

        topicEntity.Title = topic.Title;
        topicEntity.Slug = topic.Slug;
        topicEntity.Description = topic.Description;
        topicEntity.TagsJson = DomainMapping.Serialize(topic.Tags);

        return topicEntity;
    }

    private async Task<IReadOnlyDictionary<string, Guid>> UpsertFactsAsync(
        Guid topicId,
        IEnumerable<TriviaSeedFact> seedFacts,
        CancellationToken cancellationToken)
    {
        var factIdsByKey = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        foreach (var seedFact in seedFacts)
        {
            var sourceUrl = string.IsNullOrWhiteSpace(seedFact.SourceUrl) ? null : new Uri(seedFact.SourceUrl);
            var fact = Fact.Create(topicId, seedFact.Statement, seedFact.Difficulty, seedFact.Confidence, sourceUrl);
            var factEntity = await db.Facts.SingleOrDefaultAsync(
                candidate => candidate.TopicId == topicId && candidate.Statement == fact.Statement,
                cancellationToken);
            if (factEntity is null)
            {
                factEntity = new FactEntity { Id = fact.Id, TopicId = topicId };
                db.Facts.Add(factEntity);
            }

            factEntity.Statement = fact.Statement;
            factEntity.SourceUrl = fact.SourceUrl?.ToString();
            factEntity.Difficulty = (int)fact.Difficulty;
            factEntity.Confidence = fact.Confidence;

            if (!string.IsNullOrWhiteSpace(seedFact.Key))
            {
                factIdsByKey[seedFact.Key] = factEntity.Id;
            }
        }

        return factIdsByKey;
    }

    private async Task UpsertDefinitionsAsync(
        Guid topicId,
        IEnumerable<TriviaSeedDefinition> seedDefinitions,
        CancellationToken cancellationToken)
    {
        foreach (var seedDefinition in seedDefinitions)
        {
            var definition = Definition.Create(topicId, seedDefinition.Term, seedDefinition.Text, seedDefinition.Examples);
            var definitionEntity = await db.Definitions.SingleOrDefaultAsync(
                candidate => candidate.TopicId == topicId && candidate.Term == definition.Term,
                cancellationToken);
            if (definitionEntity is null)
            {
                definitionEntity = new DefinitionEntity { Id = definition.Id, TopicId = topicId };
                db.Definitions.Add(definitionEntity);
            }

            definitionEntity.Term = definition.Term;
            definitionEntity.Text = definition.Text;
            definitionEntity.ExamplesJson = DomainMapping.Serialize(definition.Examples);
        }
    }

    private async Task UpsertQuestionsAsync(
        Guid topicId,
        IEnumerable<TriviaSeedQuestion> seedQuestions,
        IReadOnlyDictionary<string, Guid> factIdsByKey,
        CancellationToken cancellationToken)
    {
        foreach (var seedQuestion in seedQuestions)
        {
            var sourceFactId = ResolveSourceFactId(seedQuestion.SourceFactKey, factIdsByKey);
            var question = Question.Create(
                    topicId,
                    seedQuestion.Prompt,
                    seedQuestion.Choices.Select(choice => AnswerChoice.Create(choice.Id, choice.Text)),
                    seedQuestion.CorrectAnswerId,
                    seedQuestion.Explanation,
                    seedQuestion.Difficulty,
                    sourceFactId)
                .MarkHumanReviewed()
                .Publish();
            var questionEntity = await db.Questions.SingleOrDefaultAsync(
                candidate => candidate.TopicId == topicId && candidate.Prompt == question.Prompt,
                cancellationToken);
            if (questionEntity is null)
            {
                questionEntity = new QuestionEntity { Id = question.Id, TopicId = topicId };
                db.Questions.Add(questionEntity);
            }

            questionEntity.Prompt = question.Prompt;
            questionEntity.ChoicesJson = DomainMapping.Serialize(question.Choices);
            questionEntity.CorrectAnswerId = question.CorrectAnswerId;
            questionEntity.Explanation = question.Explanation;
            questionEntity.Difficulty = (int)question.Difficulty;
            questionEntity.SourceFactId = question.SourceFactId;
            questionEntity.Status = (int)question.Status;
        }
    }

    private static Guid? ResolveSourceFactId(string? sourceFactKey, IReadOnlyDictionary<string, Guid> factIdsByKey)
    {
        if (string.IsNullOrWhiteSpace(sourceFactKey))
        {
            return null;
        }

        if (!factIdsByKey.TryGetValue(sourceFactKey, out var factId))
        {
            throw new InvalidOperationException($"Seed question references unknown fact key '{sourceFactKey}'.");
        }

        return factId;
    }
}

public sealed record TriviaSeedDocument(int SchemaVersion, IReadOnlyList<TriviaSeedTopic> Topics);

public sealed record TriviaSeedTopic(
    string Title,
    string Slug,
    string Description,
    IReadOnlyList<string> Tags,
    IReadOnlyList<TriviaSeedFact> Facts,
    IReadOnlyList<TriviaSeedDefinition> Definitions,
    IReadOnlyList<TriviaSeedQuestion> Questions);

public sealed record TriviaSeedFact(
    string? Key,
    string Statement,
    string? SourceUrl,
    Difficulty Difficulty,
    decimal Confidence);

public sealed record TriviaSeedDefinition(string Term, string Text, IReadOnlyList<string> Examples);

public sealed record TriviaSeedQuestion(
    string Prompt,
    IReadOnlyList<TriviaSeedAnswerChoice> Choices,
    string CorrectAnswerId,
    string Explanation,
    Difficulty Difficulty,
    string? SourceFactKey);

public sealed record TriviaSeedAnswerChoice(string Id, string Text);
