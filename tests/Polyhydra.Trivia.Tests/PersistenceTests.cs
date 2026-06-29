extern alias ApiApp;

using ApiApp::Polyhydra.Trivia.Api;
using Microsoft.EntityFrameworkCore;
using Polyhydra.Trivia.Core;
using Polyhydra.Trivia.Infrastructure;

namespace Polyhydra.Trivia.Tests;

public sealed class PersistenceTests
{
    [Fact]
    public async Task StorePersistsTopicsQuestionsSessionsAndVotesAcrossContexts()
    {
        var databasePath = GetTemporaryDatabasePath();
        try
        {
            await using (var db = CreateContext(databasePath))
            {
                await db.Database.EnsureCreatedAsync();
                var store = new TriviaStore(db);
                var topic = await store.AddTopicAsync(new CreateTopicRequest(
                    "Persistence",
                    "persistence",
                    "Durable storage trivia.",
                    ["storage"]));
                await store.AddQuestionAsync(topic.Id, new CreateQuestionRequest(
                    "Which database is used for the first local persistence slice?",
                    [
                        new CreateAnswerChoiceRequest("A", "SQLite"),
                        new CreateAnswerChoiceRequest("B", "Redis")
                    ],
                    "A",
                    "The local persistence strategy uses SQLite.",
                    Difficulty.Easy,
                    null));
                var session = await store.StartSessionAsync(new CreateGameSessionRequest(GameMode.ChatPoll, [topic.Id]));
                await store.VoteAsync(session.Id, new AnswerRequest("viewer-1", "A"));
            }

            await using (var db = CreateContext(databasePath))
            {
                var store = new TriviaStore(db);
                var topic = Assert.Single(await store.ListTopicsAsync());
                var question = Assert.Single(await store.ListQuestionsAsync(topic.Id));
                var session = Assert.Single(await db.GameSessions.ToArrayAsync());

                var results = await store.ResultsAsync(session.Id);

                Assert.Equal("persistence", topic.Slug);
                Assert.Equal("SQLite", question.Choices.Single(choice => choice.Id == "A").Text);
                Assert.NotNull(results);
                Assert.Equal(1, results.Choices.Single(choice => choice.AnswerId == "A").Count);
            }
        }
        finally
        {
            DeleteIfExists(databasePath);
        }
    }

    [Fact]
    public async Task SeedImporterLoadsVersionedJsonContent()
    {
        var databasePath = GetTemporaryDatabasePath();
        try
        {
            await using var db = CreateContext(databasePath);
            await db.Database.EnsureCreatedAsync();
            var importer = new TriviaSeedImporter(db);

            await importer.ImportDirectoryAsync(GetSeedDirectory());

            var store = new TriviaStore(db);
            var topic = Assert.Single(await store.ListTopicsAsync());
            var fact = Assert.Single(await store.ListFactsAsync(topic.Id));
            var definition = Assert.Single(await store.ListDefinitionsAsync(topic.Id));
            var question = Assert.Single(await store.ListQuestionsAsync(topic.Id));

            Assert.Equal("modern-dotnet", topic.Slug);
            Assert.Contains("cloud", fact.Statement, StringComparison.OrdinalIgnoreCase);
            Assert.Equal("Minimal API", definition.Term);
            Assert.Equal(ContentStatus.Published, question.Status);
        }
        finally
        {
            DeleteIfExists(databasePath);
        }
    }

    private static TriviaDbContext CreateContext(string databasePath)
    {
        var options = new DbContextOptionsBuilder<TriviaDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;
        return new TriviaDbContext(options);
    }

    private static string GetTemporaryDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"polyhydra-trivia-tests-{Guid.NewGuid():N}.db");

    private static string GetSeedDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "seed", "trivia");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find seed/trivia from the test output directory.");
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
