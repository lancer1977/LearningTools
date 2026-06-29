namespace Polyhydra.Trivia.Infrastructure;

public sealed class TopicEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string TagsJson { get; set; } = "[]";
}

public sealed class FactEntity
{
    public Guid Id { get; set; }

    public Guid TopicId { get; set; }

    public string Statement { get; set; } = string.Empty;

    public string? SourceUrl { get; set; }

    public int Difficulty { get; set; }

    public decimal Confidence { get; set; }
}

public sealed class DefinitionEntity
{
    public Guid Id { get; set; }

    public Guid TopicId { get; set; }

    public string Term { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public string ExamplesJson { get; set; } = "[]";
}

public sealed class QuestionEntity
{
    public Guid Id { get; set; }

    public Guid TopicId { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public string ChoicesJson { get; set; } = "[]";

    public string CorrectAnswerId { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public int Difficulty { get; set; }

    public Guid? SourceFactId { get; set; }

    public int Status { get; set; }
}

public sealed class GameSessionEntity
{
    public Guid Id { get; set; }

    public int Mode { get; set; }

    public string TopicIdsJson { get; set; } = "[]";

    public Guid CurrentQuestionId { get; set; }

    public bool IsVotingClosed { get; set; }

    public bool IsRevealed { get; set; }
}

public sealed class AnswerVoteEntity
{
    public Guid Id { get; set; }

    public Guid GameSessionId { get; set; }

    public Guid QuestionId { get; set; }

    public string VoterId { get; set; } = string.Empty;

    public string AnswerId { get; set; } = string.Empty;
}
