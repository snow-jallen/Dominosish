using MultiplayerDemo.Logic;

public class GameHost
{
    public static GameHost Instance{get;}

    static GameHost()
    {
        Instance = new GameHost();
    } 

    public List<Game> Games{get;} = new();
}