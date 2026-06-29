extern alias ApiApp;

using System.Net;
using System.Net.Http.Json;
using ApiApp::Polyhydra.Trivia.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Polyhydra.Trivia.Core;

namespace Polyhydra.Trivia.Tests;

public sealed class ApiTests : IClassFixture<WebApplicationFactory<ApiApp::Program>>
{
    private readonly HttpClient client;

    public ApiTests(WebApplicationFactory<ApiApp::Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task OpenApiDocumentIsAvailableInDevelopment()
    {
        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("/api/topics", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TopicsCanBeListedAndFetchedBySlug()
    {
        var topics = await client.GetFromJsonAsync<Topic[]>("/api/topics");

        Assert.NotNull(topics);
        var seededTopic = Assert.Single(topics);
        Assert.Equal("modern-dotnet", seededTopic.Slug);

        var response = await client.GetAsync($"/api/topics/{seededTopic.Slug}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<Topic>();
        Assert.Equal(seededTopic.Id, fetched?.Id);
    }

    [Fact]
    public async Task QuestionsCanBeListedByTopic()
    {
        var topic = Assert.Single(await client.GetFromJsonAsync<Topic[]>("/api/topics") ?? []);

        var questions = await client.GetFromJsonAsync<Question[]>($"/api/topics/{topic.Id}/questions");

        var question = Assert.Single(questions ?? []);
        Assert.Equal(topic.Id, question.TopicId);
        Assert.Equal(ContentStatus.Published, question.Status);
    }

    [Fact]
    public async Task GameSessionAcceptsAnswerAndRevealsResults()
    {
        var topic = Assert.Single(await client.GetFromJsonAsync<Topic[]>("/api/topics") ?? []);
        var createResponse = await client.PostAsJsonAsync(
            "/api/game-sessions",
            new CreateGameSessionRequest(GameMode.ChatPoll, [topic.Id]));
        createResponse.EnsureSuccessStatusCode();
        var session = await createResponse.Content.ReadFromJsonAsync<GameSession>();
        Assert.NotNull(session);

        var answerResponse = await client.PostAsJsonAsync(
            $"/api/game-sessions/{session.Id}/answer",
            new AnswerRequest("viewer-1", "A"));
        Assert.Equal(HttpStatusCode.NoContent, answerResponse.StatusCode);

        var revealResponse = await client.PostAsync($"/api/game-sessions/{session.Id}/reveal", null);
        revealResponse.EnsureSuccessStatusCode();
        var results = await revealResponse.Content.ReadFromJsonAsync<PollResults>();

        Assert.NotNull(results);
        Assert.True(results.IsRevealed);
        Assert.Equal("A", results.CorrectAnswerId);
        Assert.Equal("A", results.MajorityAnswerId);
        Assert.Equal(1, results.Choices.Single(choice => choice.AnswerId == "A").Count);
    }
}
