using System.Net.Http.Json;
using Polyhydra.Trivia.Core;

namespace Polyhydra.Trivia.Web.Services;

public sealed class TriviaApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<Topic>> GetTopicsAsync(CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<Topic[]>("/api/topics", cancellationToken) ?? Array.Empty<Topic>();

    public async Task<Topic?> GetTopicAsync(string slug, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<Topic>($"/api/topics/{slug}", cancellationToken);

    public async Task<IReadOnlyList<Question>> GetQuestionsAsync(Guid topicId, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<Question[]>($"/api/topics/{topicId}/questions", cancellationToken) ?? Array.Empty<Question>();

    public async Task<Topic> CreateTopicAsync(TopicForm form, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/topics",
            new CreateTopicRequest(form.Title, form.Slug, form.Description, SplitCsv(form.Tags)),
            cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Topic>(cancellationToken) ?? throw new InvalidOperationException("API did not return the created topic.");
    }

    public async Task<Question> CreateQuestionAsync(QuestionForm form, CancellationToken cancellationToken = default)
    {
        var request = new CreateQuestionRequest(
            form.Prompt,
            [
                new CreateAnswerChoiceRequest("A", form.ChoiceA),
                new CreateAnswerChoiceRequest("B", form.ChoiceB),
                new CreateAnswerChoiceRequest("C", form.ChoiceC),
                new CreateAnswerChoiceRequest("D", form.ChoiceD)
            ],
            form.CorrectAnswerId,
            form.Explanation,
            Difficulty.Medium,
            null);

        var response = await httpClient.PostAsJsonAsync($"/api/topics/{form.TopicId}/questions", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Question>(cancellationToken) ?? throw new InvalidOperationException("API did not return the created question.");
    }

    public async Task<GameSessionResponse> StartSessionAsync(GameMode mode, Guid[] topicIds, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/game-sessions", new CreateGameSessionRequest(mode, topicIds), cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameSessionResponse>(cancellationToken) ?? throw new InvalidOperationException("API did not return the created session.");
    }

    public async Task SubmitAnswerAsync(Guid sessionId, string voterId, string answerId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/game-sessions/{sessionId}/answer", new AnswerRequest(voterId, answerId), cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<PollResults> GetResultsAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<PollResults>($"/api/game-sessions/{sessionId}/results", cancellationToken)
        ?? throw new InvalidOperationException("API did not return poll results.");

    public async Task<PollResults> RevealAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"/api/game-sessions/{sessionId}/reveal", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PollResults>(cancellationToken)
            ?? throw new InvalidOperationException("API did not return poll results.");
    }

    private static string[] SplitCsv(string value) =>
        value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}

public sealed class TopicForm
{
    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;
}

public sealed class QuestionForm
{
    public Guid TopicId { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public string ChoiceA { get; set; } = string.Empty;

    public string ChoiceB { get; set; } = string.Empty;

    public string ChoiceC { get; set; } = string.Empty;

    public string ChoiceD { get; set; } = string.Empty;

    public string CorrectAnswerId { get; set; } = "A";

    public string Explanation { get; set; } = string.Empty;
}

public sealed record GameSessionResponse(Guid Id, GameMode Mode, IReadOnlyList<Guid> TopicIds, Guid CurrentQuestionId, bool IsVotingClosed, bool IsRevealed);

public sealed record CreateTopicRequest(string Title, string Slug, string Description, string[] Tags);

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
