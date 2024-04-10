using System.Text.Json;

namespace MultiplayerDemo.Persistence;

public class JsonHighScoreRepository : IHighScoreRepository
{
    public static IHighScoreRepository Instance { get; } = new SqliteRepository();

    private readonly string _filePath = "highscores.json";

    public JsonHighScoreRepository()
    {
    }

    public IEnumerable<HighScore> GetHighScores()
    {
        if (!File.Exists(_filePath))
        {
            return Enumerable.Empty<HighScore>();
        }

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<IEnumerable<HighScore>>(json);
    }

    public void AddHighScore(HighScore highScore)
    {
        var highScores = GetHighScores();
        highScores = highScores.Append(highScore);
        var json = JsonSerializer.Serialize(highScores);
        File.WriteAllText(_filePath, json);
    }
}
