using Polyhydra.Trivia.Api;
using Polyhydra.Trivia.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Trivia")
    ?? "Data Source=polyhydra-trivia.db";
builder.Services.AddDbContext<TriviaDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ITriviaStore, TriviaStore>();
builder.Services.AddScoped<TriviaSeedImporter>();
builder.Services.AddScoped<TriviaDatabaseInitializer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<TriviaDatabaseInitializer>();
    await initializer.InitializeAsync(app.Environment.ContentRootPath);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var topics = app.MapGroup("/api/topics");
topics.MapGet("/", async (ITriviaStore store, CancellationToken cancellationToken) =>
    Results.Ok(await store.ListTopicsAsync(cancellationToken)));
topics.MapGet("/{slug}", async (string slug, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var topic = await store.GetTopicAsync(slug, cancellationToken);
    return topic is null ? Results.NotFound() : Results.Ok(topic);
});
topics.MapPost("/", async (CreateTopicRequest request, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var topic = await store.AddTopicAsync(request, cancellationToken);
    return Results.Created($"/api/topics/{topic.Slug}", topic);
});
topics.MapPut("/{id:guid}", async (Guid id, UpdateTopicRequest request, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var topic = await store.UpdateTopicAsync(id, request, cancellationToken);
    return topic is null ? Results.NotFound() : Results.Ok(topic);
});

topics.MapGet("/{id:guid}/facts", async (Guid id, ITriviaStore store, CancellationToken cancellationToken) =>
    Results.Ok(await store.ListFactsAsync(id, cancellationToken)));
topics.MapPost("/{id:guid}/facts", async (Guid id, CreateFactRequest request, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var fact = await store.AddFactAsync(id, request, cancellationToken);
    return fact is null ? Results.NotFound() : Results.Created($"/api/topics/{id}/facts/{fact.Id}", fact);
});

topics.MapGet("/{id:guid}/definitions", async (Guid id, ITriviaStore store, CancellationToken cancellationToken) =>
    Results.Ok(await store.ListDefinitionsAsync(id, cancellationToken)));
topics.MapPost("/{id:guid}/definitions", async (Guid id, CreateDefinitionRequest request, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var definition = await store.AddDefinitionAsync(id, request, cancellationToken);
    return definition is null ? Results.NotFound() : Results.Created($"/api/topics/{id}/definitions/{definition.Id}", definition);
});

topics.MapGet("/{id:guid}/questions", async (Guid id, ITriviaStore store, CancellationToken cancellationToken) =>
    Results.Ok(await store.ListQuestionsAsync(id, cancellationToken)));
topics.MapPost("/{id:guid}/questions", async (Guid id, CreateQuestionRequest request, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var question = await store.AddQuestionAsync(id, request, cancellationToken);
    return question is null ? Results.NotFound() : Results.Created($"/api/topics/{id}/questions/{question.Id}", question);
});

var sessions = app.MapGroup("/api/game-sessions");
sessions.MapPost("/", async (CreateGameSessionRequest request, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var session = await store.StartSessionAsync(request, cancellationToken);
    return Results.Created($"/api/game-sessions/{session.Id}", session);
});
sessions.MapGet("/{id:guid}", async (Guid id, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var session = await store.GetSessionAsync(id, cancellationToken);
    return session is null ? Results.NotFound() : Results.Ok(session);
});
sessions.MapPost("/{id:guid}/answer", async (Guid id, AnswerRequest request, ITriviaStore store, CancellationToken cancellationToken) =>
    await store.VoteAsync(id, request, cancellationToken) ? Results.NoContent() : Results.NotFound());
sessions.MapPost("/{id:guid}/reveal", async (Guid id, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var results = await store.RevealAsync(id, cancellationToken);
    return results is null ? Results.NotFound() : Results.Ok(results);
});
sessions.MapGet("/{id:guid}/results", async (Guid id, ITriviaStore store, CancellationToken cancellationToken) =>
{
    var results = await store.ResultsAsync(id, cancellationToken);
    return results is null ? Results.NotFound() : Results.Ok(results);
});

app.Run();

public partial class Program;
