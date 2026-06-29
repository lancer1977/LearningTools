using Polyhydra.Trivia.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TriviaStore>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var topics = app.MapGroup("/api/topics");
topics.MapGet("/", (TriviaStore store) => Results.Ok(store.ListTopics()));
topics.MapGet("/{slug}", (string slug, TriviaStore store) =>
{
    var topic = store.GetTopic(slug);
    return topic is null ? Results.NotFound() : Results.Ok(topic);
});
topics.MapPost("/", (CreateTopicRequest request, TriviaStore store) =>
{
    var topic = store.AddTopic(request);
    return Results.Created($"/api/topics/{topic.Slug}", topic);
});
topics.MapPut("/{id:guid}", (Guid id, UpdateTopicRequest request, TriviaStore store) =>
{
    var topic = store.UpdateTopic(id, request);
    return topic is null ? Results.NotFound() : Results.Ok(topic);
});

topics.MapGet("/{id:guid}/facts", (Guid id, TriviaStore store) => Results.Ok(store.ListFacts(id)));
topics.MapPost("/{id:guid}/facts", (Guid id, CreateFactRequest request, TriviaStore store) =>
{
    var fact = store.AddFact(id, request);
    return fact is null ? Results.NotFound() : Results.Created($"/api/topics/{id}/facts/{fact.Id}", fact);
});

topics.MapGet("/{id:guid}/definitions", (Guid id, TriviaStore store) => Results.Ok(store.ListDefinitions(id)));
topics.MapPost("/{id:guid}/definitions", (Guid id, CreateDefinitionRequest request, TriviaStore store) =>
{
    var definition = store.AddDefinition(id, request);
    return definition is null ? Results.NotFound() : Results.Created($"/api/topics/{id}/definitions/{definition.Id}", definition);
});

topics.MapGet("/{id:guid}/questions", (Guid id, TriviaStore store) => Results.Ok(store.ListQuestions(id)));
topics.MapPost("/{id:guid}/questions", (Guid id, CreateQuestionRequest request, TriviaStore store) =>
{
    var question = store.AddQuestion(id, request);
    return question is null ? Results.NotFound() : Results.Created($"/api/topics/{id}/questions/{question.Id}", question);
});

var sessions = app.MapGroup("/api/game-sessions");
sessions.MapPost("/", (CreateGameSessionRequest request, TriviaStore store) =>
{
    var session = store.StartSession(request);
    return Results.Created($"/api/game-sessions/{session.Id}", session);
});
sessions.MapGet("/{id:guid}", (Guid id, TriviaStore store) =>
{
    var session = store.GetSession(id);
    return session is null ? Results.NotFound() : Results.Ok(session);
});
sessions.MapPost("/{id:guid}/answer", (Guid id, AnswerRequest request, TriviaStore store) =>
    store.Vote(id, request) ? Results.NoContent() : Results.NotFound());
sessions.MapPost("/{id:guid}/reveal", (Guid id, TriviaStore store) =>
{
    var results = store.Reveal(id);
    return results is null ? Results.NotFound() : Results.Ok(results);
});
sessions.MapGet("/{id:guid}/results", (Guid id, TriviaStore store) =>
{
    var results = store.Results(id);
    return results is null ? Results.NotFound() : Results.Ok(results);
});

app.Run();

public partial class Program;
