
global using MultiplayerDemo.Logic.Exceptions;

namespace MultiplayerDemo.Logic;

public class Game
{
    public static Game Instance { get; } = new Game();

    public Player? Player1 { get; private set; }
    public Player? Player2 { get; private set; }
    public List<Tile> Board { get; private set; }

    public Game()
    {
        Reset();
    }

    public bool IsPlayable => Player1 is not null && Player2 is not null && !IsGameOver;
    public bool IsGameOver => Player1.Tiles.Count == 0 || Player2.Tiles.Count == 0 || NoOneCanPlay;
    public bool NoOneCanPlay => !Player1.HasMatchFor(Board.Last()) && !Player2.HasMatchFor(Board.Last());
    public Player? Winner => !IsGameOver ? null : (Player1.Tiles.Count < Player2.Tiles.Count ? Player1 : Player2);

    public void Join(string newPlayerName)
    {
        Join(new Player(newPlayerName));
    }

    public void Join(Player player)
    {
        if (Player1 is null)
        {
            Player1 = player;
        }
        else if (Player2 is null)
        {
            Player2 = player;
        }
        else
        {
            throw new GameFullException();
        }
        GameStateChanged?.Invoke();
    }

    public void Reset()
    {
        Player1 = null;
        Player2 = null;
        Board = new List<Tile> { new Tile(1, 1) };
    }

    public void PlayTile(Player newPlayer1, Tile tileToPlay)
    {
        if (newPlayer1.Tiles.Contains(tileToPlay) == false)
        {
            throw new InvalidMoveException();
        }

        var numtomatch = Board.Last().Num2;

        if (tileToPlay.Num1 == numtomatch)
        {
            Board.Add(tileToPlay);
            newPlayer1.Tiles.Remove(tileToPlay);
        }
        else if (tileToPlay.Num2 == numtomatch)
        {
            newPlayer1.Tiles.Remove(tileToPlay);
            Board.Add(new Tile(tileToPlay.Num2, tileToPlay.Num1));
        }
        else
        { throw new InvalidMoveException(); }

        GameStateChanged?.Invoke();
    }

    public event Action? GameStateChanged;

}