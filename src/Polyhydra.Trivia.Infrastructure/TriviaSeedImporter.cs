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

            await db.SaveChangesAsync(cancellationToken);

            var factIdsByKey = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            foreach (var seedFact in seedTopic.Facts)
            {
                var sourceUrl = string.IsNullOrWhiteSpace(seedFact.SourceUrl) ? null : new Uri(seedFact.SourceUrl);
                var fact = Fact.Create(topicEntity.Id, seedFact.Statement, seedFact.Difficulty, seedFact.Confidence, sourceUrl);
                var factEntity = await db.Facts.SingleOrDefaultAsync(
                    candidate => candidate.TopicId == topicEntity.Id && candidate.Statement == fact.Statement,
                    cancellationToken);
                if (factEntity is null)
                {
                    factEntity = new FactEntity { Id = fact.Id, TopicId = topicEntity.Id };
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

            foreach (var seedDefinition in seedTopic.Definitions)
            {
                var definition = Definition.Create(topicEntity.Id, seedDefinition.Term, seedDefinition.Text, seedDefinition.Examples);
                var definitionEntity = await db.Definitions.SingleOrDefaultAsync(
                    candidate => candidate.TopicId == topicEntity.Id && candidate.Term == definition.Term,
                    cancellationToken);
                if (definitionEntity is null)
                {
                    definitionEntity = new DefinitionEntity { Id = definition.Id, TopicId = topicEntity.Id };
                    db.Definitions.Add(definitionEntity);
                }

                definitionEntity.Term = definition.Term;
                definitionEntity.Text = definition.Text;
                definitionEntity.ExamplesJson = DomainMapping.Serialize(definition.Examples);
            }

            foreach (var seedQuestion in seedTopic.Questions)
            {
                var sourceFactId = ResolveSourceFactId(seedQuestion.SourceFactKey, factIdsByKey);
                var question = Question.Create(
                        topicEntity.Id,
                        seedQuestion.Prompt,
                        seedQuestion.Choices.Select(choice => AnswerChoice.Create(choice.Id, choice.Text)),
                        seedQuestion.CorrectAnswerId,
                        seedQuestion.Explanation,
                        seedQuestion.Difficulty,
                        sourceFactId)
                    .MarkHumanReviewed()
                    .Publish();
                var questionEntity = await db.Questions.SingleOrDefaultAsync(
                    candidate => candidate.TopicId == topicEntity.Id && candidate.Prompt == question.Prompt,
                    cancellationToken);
                if (questionEntity is null)
                {
                    questionEntity = new QuestionEntity { Id = question.Id, TopicId = topicEntity.Id };
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

        await db.SaveChangesAsync(cancellationToken);
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
