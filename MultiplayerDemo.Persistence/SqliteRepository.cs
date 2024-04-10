using SQLite;

namespace MultiplayerDemo.Persistence;

public class SqliteRepository : IHighScoreRepository
{
    private SQLiteConnection db;

    public SqliteRepository()
    {
        db = new SQLiteConnection("scores.sqlite");
        db.CreateTable<HighScore>();
    }

    public void AddHighScore(HighScore highScore)
    {
        db.Insert(highScore);
    }

    public IEnumerable<HighScore> GetHighScores()
    {        
        return db.Table<HighScore>();
    }
}
