
namespace MultiplayerDemo.Logic;

public class Player
{
    public const int StartingTileCount = 7;
    private readonly string _name;
    public string Name => _name;

    public List<Tile> Tiles { get; private set; } = new();

    public Player(string name, int tileCount = StartingTileCount)
    {
        _name = name;
        Tiles.AddRange(Enumerable.Range(0, tileCount).Select(_ => new Tile()));
    }

    internal bool HasMatchFor(Tile other) =>
        Tiles.Any(t => t.Num1 == other.Num2 || t.Num2 == other.Num2);
}
