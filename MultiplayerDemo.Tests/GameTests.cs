namespace MultiplayerDemo.Tests;

public class GameTests
{
    public Game Game { get; }

    public GameTests()
    {
        Game = new Game();
    }

    [Fact]
    public void NewPlayersStartOffWithXTiles()
    {
        var newPlayer = new Player("Dallan", 7);
        Assert.Equal(7, newPlayer.Tiles.Count);
        newPlayer.Name.Should().Be("Dallan");
    }

    [Fact]
    public void NewGameWithOnePlayerNotPlayable()
    {
        Game.Reset();
        var newPlayer = new Player("Dallan", 7);
        Game.Join(newPlayer);
        Assert.False(Game.IsPlayable);
    }

    [Fact]
    public void NewGameWithTwoPlayersIsPlayable()
    {
        Game.Reset();
        var newPlayer1 = new Player("Dallan", 7);
        var newPlayer2 = new Player("Jonathan", 7);
        Game.Join(newPlayer1);
        Game.Join(newPlayer2);
        Assert.True(Game.IsPlayable);
    }

    [Fact]
    public void ResettingGameReinitializesTheBoardWithASingleStartingTile()
    {
        Game.Reset();
        Game.Board.Count.Should().Be(1);
        Game.Board.Last().Should().BeEquivalentTo(new Tile(1, 1));
    }

    [Fact]
    public void NewTileMakesARandomTileBetweenOneAndMax()
    {
        var tile = new Tile();
        Assert.True(tile.Num1 >= 1 && tile.Num1 <= Tile.Max);
        Assert.True(tile.Num2 >= 1 && tile.Num2 <= Tile.Max);
    }
}
