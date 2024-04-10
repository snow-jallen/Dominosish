namespace MultiplayerDemo.Persistence;

public interface IHighScoreRepository
{
    IEnumerable<HighScore> GetHighScores();
    void AddHighScore(HighScore highScore);
}
