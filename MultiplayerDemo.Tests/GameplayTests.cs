namespace MultiplayerDemo.Tests;

public class GameplayTests
{
    private Player player1;
    private Player player2;
    private Game Game;

    public GameplayTests()
    {
        Game = new Game();
        Game.Reset();
        player1 = new Player("Dallan", 7);
        Game.Join(player1);
        player2 = new Player("Jonathan", 7);
        Game.Join(player2);
    }

    [Fact]
    public void Player1CanPlaceATile()
    {
        var tileToPlay = new Tile(1, Game.Board.First().Num1);
        player1.Tiles.Add(tileToPlay);
        Game.PlayTile(player1, tileToPlay);

        Game.Board.Count.Should().Be(2);
        Game.Board.Last().Num2.Should().Be(1);
    }

    [Fact]
    public void PlayerCannotPlayATileThatDoesNotHave()
    {
        var tileToPlay = new Tile(1, 2);
        player1.Tiles.Clear();
        player1.Tiles.Add(new Tile(2, 2));
        Assert.Throws<InvalidMoveException>(() => Game.PlayTile(player1, tileToPlay));
    }

    [Fact]
    public void PlayerCannotPlayATileThatDoesntHaveRightNumber()
    {
        var tileToPlay = new Tile(2, 3);
        player1.Tiles.Clear();
        player1.Tiles.Add(tileToPlay);
        Assert.Throws<InvalidMoveException>(() => Game.PlayTile(player1, tileToPlay));
    }

    [Fact]
    public void Player1Wins()
    {
        var tileToPlay = new Tile(1, Game.Board.First().Num1);
        player1.Tiles.Clear();
        player1.Tiles.Add(new Tile(1, 2));
        player1.Tiles.Add(new Tile(2, 3));
        player1.Tiles.Add(new Tile(3, 4));
        var expectedBoardLength = 1 + player1.Tiles.Count;

        while (player1.Tiles.Any())
        {
            Game.PlayTile(player1, player1.Tiles.First());
        }

        Game.Board.Count.Should().Be(expectedBoardLength);
        Game.IsPlayable.Should().BeFalse();
        Game.Winner.Should().Be(player1);
        Game.IsGameOver.Should().BeTrue();
    }
}
