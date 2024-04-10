using SQLite;

namespace MultiplayerDemo.Persistence;

public class HighScore
{
    [PrimaryKey, AutoIncrement]
    public int Id{get;set;}
    public string Name { get; set; }
    public int Score { get; set; }
    public DateTime Timestamp { get; set; }
    public string Quote { get; set; }
}
