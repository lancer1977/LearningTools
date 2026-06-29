using Polyhydra.Trivia.Core;

namespace Polyhydra.Trivia.Api;

public sealed class TriviaStore
{
    private readonly List<Topic> topics = [];
    private readonly List<Fact> facts = [];
    private readonly List<Definition> definitions = [];
    private readonly List<Question> questions = [];
    private readonly List<GameSession> sessions = [];

    public TriviaStore()
    {
        var dotnet = Topic.Create(
            "Modern .NET",
            "modern-dotnet",
            "Questions about current .NET platform fundamentals.",
            ["dotnet", "platform"]);
        topics.Add(dotnet);

        var fact = Fact.Create(
            dotnet.Id,
            ".NET supports cloud, web, desktop, mobile, gaming, IoT, and AI workloads.",
            Difficulty.Easy,
            0.95m,
            new Uri("https://dotnet.microsoft.com/"));
        facts.Add(fact);

        definitions.Add(Definition.Create(
            dotnet.Id,
            "Minimal API",
            "A compact ASP.NET Core style for defining HTTP endpoints directly in code.",
            ["MapGet", "MapPost"]));

        questions.Add(Question.Create(
                dotnet.Id,
                "Which ASP.NET Core style maps HTTP endpoints directly in Program.cs?",
                [
                    AnswerChoice.Create("A", "Minimal APIs"),
                    AnswerChoice.Create("B", "Windows Forms"),
                    AnswerChoice.Create("C", "XAML resources"),
                    AnswerChoice.Create("D", "MSBuild targets")
                ],
                "A",
                "Minimal APIs let an ASP.NET Core app map routes directly through methods such as MapGet and MapPost.",
                Difficulty.Easy,
                fact.Id)
            .MarkHumanReviewed()
            .Publish());
    }

    public IReadOnlyList<Topic> ListTopics() => topics;

    public Topic? GetTopic(string slug) =>
        topics.SingleOrDefault(topic => string.Equals(topic.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public Topic AddTopic(CreateTopicRequest request)
    {
        var topic = Topic.Create(request.Title, request.Slug, request.Description, request.Tags);
        topics.Add(topic);
        return topic;
    }

    public Topic? UpdateTopic(Guid id, UpdateTopicRequest request)
    {
        var index = topics.FindIndex(topic => topic.Id == id);
        if (index < 0)
        {
            return null;
        }

        var updated = Topic.Create(request.Title, request.Slug, request.Description, request.Tags) with { Id = id };
        topics[index] = updated;
        return updated;
    }

    public IReadOnlyList<Fact> ListFacts(Guid topicId) =>
        facts.Where(fact => fact.TopicId == topicId).ToArray();

    public Fact? AddFact(Guid topicId, CreateFactRequest request)
    {
        if (!topics.Any(topic => topic.Id == topicId))
        {
            return null;
        }

        var sourceUrl = string.IsNullOrWhiteSpace(request.SourceUrl) ? null : new Uri(request.SourceUrl);
        var fact = Fact.Create(topicId, request.Statement, request.Difficulty, request.Confidence, sourceUrl);
        facts.Add(fact);
        return fact;
    }

    public IReadOnlyList<Definition> ListDefinitions(Guid topicId) =>
        definitions.Where(definition => definition.TopicId == topicId).ToArray();

    public Definition? AddDefinition(Guid topicId, CreateDefinitionRequest request)
    {
        if (!topics.Any(topic => topic.Id == topicId))
        {
            return null;
        }

        var definition = Definition.Create(topicId, request.Term, request.Text, request.Examples);
        definitions.Add(definition);
        return definition;
    }

    public IReadOnlyList<Question> ListQuestions(Guid topicId) =>
        questions.Where(question => question.TopicId == topicId).ToArray();

    public Question? AddQuestion(Guid topicId, CreateQuestionRequest request)
    {
        if (!topics.Any(topic => topic.Id == topicId))
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
        questions.Add(question);
        return question;
    }

    public GameSession StartSession(CreateGameSessionRequest request)
    {
        var topicIds = request.TopicIds.Length == 0 ? topics.Select(topic => topic.Id).ToArray() : request.TopicIds;
        var question = questions.First(candidate => topicIds.Contains(candidate.TopicId));
        var session = GameSession.Start(request.Mode, topicIds, question.Id);
        sessions.Add(session);
        return session;
    }

    public GameSession? GetSession(Guid id) => sessions.SingleOrDefault(session => session.Id == id);

    public bool Vote(Guid sessionId, AnswerRequest request)
    {
        var session = GetSession(sessionId);
        if (session is null)
        {
            return false;
        }

        session.Vote(request.VoterId, request.AnswerId);
        return true;
    }

    public PollResults? Reveal(Guid sessionId)
    {
        var session = GetSession(sessionId);
        if (session is null)
        {
            return null;
        }

        session.Reveal();
        return Results(session);
    }

    public PollResults? Results(Guid sessionId)
    {
        var session = GetSession(sessionId);
        return session is null ? null : Results(session);
    }

    private PollResults Results(GameSession session)
    {
        var question = questions.Single(question => question.Id == session.CurrentQuestionId);
        return session.GetResults(question);
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
