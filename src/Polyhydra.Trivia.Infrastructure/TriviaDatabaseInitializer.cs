using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Polyhydra.Trivia.Infrastructure;

public sealed class TriviaDatabaseInitializer(
    TriviaDbContext db,
    TriviaSeedImporter seedImporter,
    ILogger<TriviaDatabaseInitializer> logger)
{
    public async Task InitializeAsync(string contentRoot, CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);

        var seedDirectory = ResolveSeedDirectory(contentRoot);
        if (seedDirectory is null)
        {
            logger.LogWarning("Trivia seed directory was not found. Skipping seed import.");
            return;
        }

        await seedImporter.ImportDirectoryAsync(seedDirectory, cancellationToken);
    }

    private static string? ResolveSeedDirectory(string contentRoot)
    {
        var candidates = new[]
        {
            Path.Combine(contentRoot, "seed", "trivia"),
            Path.GetFullPath(Path.Combine(contentRoot, "..", "..", "seed", "trivia")),
            Path.Combine(AppContext.BaseDirectory, "seed", "trivia")
        };

        return candidates.FirstOrDefault(Directory.Exists);
    }
}
