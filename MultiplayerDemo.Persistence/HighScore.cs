using System.Text.Json;

namespace MultiplayerDemo.Persistence;

public class HighScore
{
    public string Name { get; set; }
    public int Score { get; set; }
    public DateTime Timestamp { get; set; }
    public string Quote { get; set; }
}

public interface IHighScoreRepository
{
    Task<IEnumerable<HighScore>> GetHighScoresAsync();
    Task AddHighScoreAsync(HighScore highScore);
}

public class JsonHighScoreRepository : IHighScoreRepository
{
    public static IHighScoreRepository Instance { get; } = new JsonHighScoreRepository();

    private readonly string _filePath = "highscores.json";

    public JsonHighScoreRepository()
    {
    }

    public async Task<IEnumerable<HighScore>> GetHighScoresAsync()
    {
        if (!File.Exists(_filePath))
        {
            return Enumerable.Empty<HighScore>();
        }

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<IEnumerable<HighScore>>(json);
    }

    public async Task AddHighScoreAsync(HighScore highScore)
    {
        var highScores = await GetHighScoresAsync();
        highScores = highScores.Append(highScore);
        var json = JsonSerializer.Serialize(highScores);
        await File.WriteAllTextAsync(_filePath, json);
    }
}
