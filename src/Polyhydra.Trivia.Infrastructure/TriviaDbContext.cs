using Microsoft.EntityFrameworkCore;

namespace Polyhydra.Trivia.Infrastructure;

public sealed class TriviaDbContext(DbContextOptions<TriviaDbContext> options) : DbContext(options)
{
    public DbSet<TopicEntity> Topics => Set<TopicEntity>();

    public DbSet<FactEntity> Facts => Set<FactEntity>();

    public DbSet<DefinitionEntity> Definitions => Set<DefinitionEntity>();

    public DbSet<QuestionEntity> Questions => Set<QuestionEntity>();

    public DbSet<GameSessionEntity> GameSessions => Set<GameSessionEntity>();

    public DbSet<AnswerVoteEntity> AnswerVotes => Set<AnswerVoteEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TopicEntity>(entity =>
        {
            entity.HasKey(topic => topic.Id);
            entity.HasIndex(topic => topic.Slug).IsUnique();
            entity.Property(topic => topic.Title).IsRequired();
            entity.Property(topic => topic.Slug).IsRequired();
            entity.Property(topic => topic.Description).IsRequired();
            entity.Property(topic => topic.TagsJson).IsRequired();
        });

        modelBuilder.Entity<FactEntity>(entity =>
        {
            entity.HasKey(fact => fact.Id);
            entity.HasIndex(fact => new { fact.TopicId, fact.Statement }).IsUnique();
            entity.Property(fact => fact.Statement).IsRequired();
            entity.HasOne<TopicEntity>()
                .WithMany()
                .HasForeignKey(fact => fact.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DefinitionEntity>(entity =>
        {
            entity.HasKey(definition => definition.Id);
            entity.HasIndex(definition => new { definition.TopicId, definition.Term }).IsUnique();
            entity.Property(definition => definition.Term).IsRequired();
            entity.Property(definition => definition.Text).IsRequired();
            entity.Property(definition => definition.ExamplesJson).IsRequired();
            entity.HasOne<TopicEntity>()
                .WithMany()
                .HasForeignKey(definition => definition.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionEntity>(entity =>
        {
            entity.HasKey(question => question.Id);
            entity.HasIndex(question => new { question.TopicId, question.Prompt }).IsUnique();
            entity.Property(question => question.Prompt).IsRequired();
            entity.Property(question => question.ChoicesJson).IsRequired();
            entity.Property(question => question.CorrectAnswerId).IsRequired();
            entity.Property(question => question.Explanation).IsRequired();
            entity.HasOne<TopicEntity>()
                .WithMany()
                .HasForeignKey(question => question.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GameSessionEntity>(entity =>
        {
            entity.HasKey(session => session.Id);
            entity.Property(session => session.TopicIdsJson).IsRequired();
        });

        modelBuilder.Entity<AnswerVoteEntity>(entity =>
        {
            entity.HasKey(vote => vote.Id);
            entity.HasIndex(vote => new { vote.GameSessionId, vote.QuestionId, vote.VoterId }).IsUnique();
            entity.Property(vote => vote.VoterId).IsRequired();
            entity.Property(vote => vote.AnswerId).IsRequired();
            entity.HasOne<GameSessionEntity>()
                .WithMany()
                .HasForeignKey(vote => vote.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
