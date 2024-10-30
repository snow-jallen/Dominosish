# Multi-Player Dominos(ish)

## Objectives
- Practice the mechanics necessary to let multiple users play the same Game
- Practice interactivity in the browser
- Be introduced to events, allowing objects to communicate with each other
- Practice working with multiple pages at different URLs

## Game Scenario

We're going to build a very simple live-action dominos game, where each player can play as quickly as they can, as long as they have a valid play to make.  Someone wins when they are the first to run out of tiles. If neither player can make a move then the player that has the fewest tiles remaining in their hand wins.

## Programming

> Note: If you come across a custom exception (e.g. InvalidMoveException, or something similar) and you get a red squiggle under that name saying it doesn't exist... click on the red squiggle and press Ctrl+. (or click on the light bulb), and then click 'Generate Class'.

### The Tile Object

1. The most basic part of this game is the individual tile.  Let's make a `Tile` class in the Logic project, and use the following code for it.  
   ```csharp
   public class Tile : IEquatable<Tile?>
   {

      public const int Max = 6;

      public Tile(int v1, int v2)
      {
         Num1 = v1;
         Num2 = v2;
      }

      public Tile()
      {
         Num1 = Random.Shared.Next(1, Max + 1);
         Num2 = Random.Shared.Next(1, Max + 1);
      }

      public int Num1 { get; }
      public int Num2 { get; }

      public override bool Equals(object? obj)
      {
         return Equals(obj as Tile);
      }

      public bool Equals(Tile? other)
      {
         return other is not null &&
                  (
                     (Num1 == other.Num1 && Num2 == other.Num2) ||
                     (Num1 == other.Num2 && Num2 == other.Num1)
                  );
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(Num1, Num2);
      }

      public static bool operator ==(Tile? left, Tile? right)
      {
         return EqualityComparer<Tile>.Default.Equals(left, right);
      }

      public static bool operator !=(Tile? left, Tile? right)
      {
         return !(left == right);
      }

      public override string ToString() => $"[{Num1}|{Num2}]";
   }
   ```
1. Note how we are overriding the Equals and comparison methods.  Add the following tests to your Test project to ensure that we can easily compare two different tile instances and see if they're equivalent (even though they are two different references to two different objects in the heap).
   ```csharp
    [Fact]
    public void OppositeTilesAreEqual()
    {
        var t1 = new Tile(1, 2);
        var t2 = new Tile(2, 1);
        t1.Equals(t2).Should().BeTrue();
    }
   ```
1. **Commit.**

### The Player Object
1. We need an object that can represent each player and the tiles in their hand.  
   ```csharp
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
   ```

### The Game Object
1. Now let's make an object that will actually play the game.  Make a `Game.cs` file in your Logic project with a Game class.  
1. Add a public static property named `Instance` of type `Game` with a getter and a private setter, and initialize that to a new Game instance.
1. Add properties for Player1 and Player2 of type Player.  Make them public properties with a private setter.
1. Add a public property named `Board` of type `List<Tile>`, again only include a private setter.
1. Add the following property to the `Game` class:
   ```csharp
    public bool NoOneCanPlay
    {
        get
        {
            if (Player1 is null || Player2 is null)
                return false;

            var player1CanPlay = Player1.HasMatchFor(Board.Last());
            var player2CanPlay = Player2.HasMatchFor(Board.Last());
            return !player1CanPlay && !player2CanPlay;
        }
    }   
   ```
1. **Commit.**
1. Create a test that makes a Game instance, and verifies that `IsGameOver` is false.  Create the `IsGameOver` property with a getter that returns true if Player 1 has 0 tiles, if Player 2 has 0 tiles, or if `NoOneCanPlay` is true.
   > Note that you'll have to be careful if Player1 or Player2 is null.  How can you handle that so your test passes without throwing a null reference exception?
1. Create a new test that makes a Game instance, and verifies that `IsPlayable` is false.  Create the `IsPlayable` property in the Game object where the body of the property verifies that Player1 is not null, Player2 is not null, and IsGameOver is false.
1. Paste the following code into the Game class.  This adds two 'events', the GameReset event and the GameStateChanged event, which are ways that this object can tell other object that something special happened.  

It also adds a constructor that calls Reset(), which initializes the players and the board.

It also adds the Join() method to let a player join the game (either as Player1 or Player2, depending on who joins first).
   ```csharp    
    public event Action? GameReset;
    public event Action? GameStateChanged;

    public Game()
    {
        Reset();
    }
    
    public void Reset()
    {
        Player1 = null;
        Player2 = null;
        Board = new List<Tile> { new Tile(1, 1) };
        GameReset?.Invoke();
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
   ```
1. Create a new test that makes a Game, calls `Join` once, and verifies that IsPlayable is false.
1. Create a new test that makes a Game, calls `Join` twice (passing in a new Player instance each time), and verifies that IsPlayable is true, and IsGameOver is false.
1. **Commit.**
1. Now our Game object needs a way to let a player play a tile.  Paste the following code into your Game object:
   ```csharp
    public void PlayTile(Player player, Tile tile)
    {
        if (player.Tiles.Contains(tile) == false)
        {
            throw new InvalidMoveException();
        }

        var numtomatch = Board.Last().Num2;

        if (tile.Num1 == numtomatch)
        {
            Board.Add(tile);
            player.Tiles.Remove(tile);
        }
        else if (tile.Num2 == numtomatch)
        {
            player.Tiles.Remove(tile);
            Board.Add(new Tile(tile.Num2, tile.Num1));
        }
        else
        { throw new InvalidMoveException(); }

        GameStateChanged?.Invoke();
    }   
   ```
1. Add a test that tests a player can play a tile on the board.  `var tileToPlay = new Tile(1, Game.Board.First().Num1);` will create a new tile that can be played on the board.  Then you can call `player1.Tiles.Add(tileToPlay);` to add that new tile to the player's hand.  Now call PlayTile and verify that the count of the tiles in the Board is 2, and that the last tile in the board matches `tileToPlay`.
1. Copy the last test, but delete the line where you add `tileToPlay` to the player's Tiles collection (so the tile is not part of the player's hand).  Now change the test to ensure that when you call PlayTile an InvalidMoveException is thrown.
1. **Commit.**
1. Add a new property to the Game class named Winner of type `Player?`.  In the getter, just return null (for now).
1. Add the following test:
   ```csharp
    [Fact]
    public void Player1Wins()
    {
        var game = new Game();
        var player1 = new Player("P1", 7);
        var player2 = new Player("P2", 7);
        game.Join(player1);
        game.Join(player2);

        player1.Tiles.Clear();
        player1.Tiles.Add(new Tile(1, 2));
        player1.Tiles.Add(new Tile(2, 3));
        player1.Tiles.Add(new Tile(3, 4));
        var expectedBoardLength = 1 + player1.Tiles.Count;

        while (player1.Tiles.Any())
        {
            game.PlayTile(player1, player1.Tiles.First());
        }

        game.Board.Count.Should().Be(expectedBoardLength);
        game.IsPlayable.Should().BeFalse();
        game.Winner.Should().Be(player1);
        game.IsGameOver.Should().BeTrue();
    }   
   ```
1. Add logic to the Winner property to first check if IsGameOver is true or not.  If IsGameOver is false, return null (because there is no winner).  If IsGameOver is true, return whichever player has the least number of tiles.  Player1 wins any ties.  Once you implement that correctly the test should pass.

### Web User Interface

#### Home Page
A few things need to happen on the home page.  The first thing we should check is to see if there's room to join the current game or not.

1. In the code block of `Home.razor` add a string to store the new player's name (`string newPlayerName;`).  Then add some markup that checks if player 2 is null (that tells us if there's still room to join the game).  
   ```razor
   @if(Game.Instance.Player2 is null)
   {
      <input @bind=newPlayerName placeholder="Your Name" />
      <button @onclick=joinGame>Join Game</button>
   }
   else
   {
      <p>Sorry, looks like the game is full</p>
      <p>@Game.Instance.Player1.Name is already playing @Game.Instance.Player2.Name</p>
   }
   ```
1. Make a method `joinGame` that will call `Game.Instance.Join(new Player(newPlayerName));` However, in order to access the `Game` class you'll need to add `@using Lab12Logic` (or whatever your namespace is) to the top of the file.
1. Once that player joins the game, we want them to go to another page where they can actually play the game.  There's a special class called `NavigationManager` that Blazor gives us to make moving between pages pretty easy.  
1. At the very top of the file, add `@inject NavigationManager navManager`, then at the end of the `joinGame()` method, call `navManager.NavigateTo($"/play?playerName={newPlayerName}");`

#### Play Page
You may have noticed that there is no page at "/play".  Good job. :)
1. Create a new file named `Play.razor` right next to `Home.razor`
1. At the very top of the file, add the following lines:
   ```razor
   @page "/play"
   @inject NavigationManager navManager   
   @using Lab12Demo.Logic @*or whatever namespace you used*@
   ```
1. Add a code block at the bottom, with the following:
   ```razor
    Game game = Game.Instance;
    Player? me;
    Player? other;
    List<string> errors = new();

    [SupplyParameterFromQuery]      //Note(1)
    public string PlayerName { get; set; }

    protected override void OnParametersSet()   //Note(2)
    {
        if(game.Player1?.Name == PlayerName)
        {
            me = game.Player1;
        }
        else if(game.Player2?.Name == PlayerName)
        {
            me = game.Player2;
        }
        other = game.Player1 == me ? game.Player2 : game.Player1;
        errors.Clear();
    }      
   ```
   This code does a few interesting things.
   - **Note(1):** This tells blazor to look in the URL and take whatever comes after `?playerName=` and assign that into the `PlayerName` property.  So if the URL up in the address bar is `/play?playerName=Frank` then the `PlayerName` property would have the value `Frank`.
   - **Note(2):** The `OnParametersSet()` method is a special method that is run once any page-level parameters (like `PlayerName`) have been set.  In this method we determine which player we are and who the other player is.
1. Now let's add a bit of display logic to the Play page.  There are three main states the page can be in:
   - Missing or invalid player name: show an error message and that's it.
   - Game Over - tell the user the game is over
   - If we have a valid player and the game isn't over, then we should let the user play! 
   ```razor
   @if (me is null)
   {
      <div>There is no player @PlayerName</div>
   }
   else if(game.IsGameOver)
   {
      <h3>Game Over</h3>
   }
   else //we're playing
   {
      <h3>Play for @me.Name</h3>    
   }   
   ```
   Try running `dotnet watch` in the Web/Blazor directory (whatever you called yours), and try to join a game.  You should automatically be redirected to the /play page and it *should* say "Play for Fred" (or whatever your name is).
1. **Commit.**
1. Create three columns to:
   - Show your player's pieces
   - Show what's on the game board
   - Show the other player's pieces (but just show "(X, X)" don't show the actual numbers)
1. A bit of css to show the boxes
   ```css
   <style>
      h4 {
         margin-top: 0px;
      }
      .column {
         display: inline-block;
         vertical-align: top;
         min-width: 15%;
         max-width: 33%;
         border-width: 2px;
         border-radius: 10px;
         border-style: solid;
         padding: 10px;
      }
   </style>   
   ```
1. Content of the columns:
   ```csharp
    <div class="column">
        <h4>My Tiles</h4>
        @foreach (var tile in me.Tiles)
        {
            <p @onclick=@(()=>playTile(tile))>(@tile.Num1, @tile.Num2)</p>
        }
    </div>
    <div class="column">
        <h4>Game Board</h4>
        @foreach (var tile in game.Board)
        {
            <p>(@tile.Num1, @tile.Num2)</p>
        }
    </div>
    <div class="column">
        @if (other is null)
        {
            <h4>Waiting for another player...</h4>
        }
        else
        {
            <h4>@other.Name's Tiles</h4>
            foreach (var tile in other.Tiles)
            {
                <p>(X, X)</p>
            }
        }
    </div>
   ```
1. Which means we need the `playTile` method
   ```razor
    void playTile(Tile t)
    {
        try
        {
            game.PlayTile(me, t);
        }
        catch (InvalidMoveException)
        {
            errors.Add($"You cannot play {t} on {game.Board.Last()}");
        }
    }   
   ```
#### Live Updates
1. At this point you can play the game, but you only see your moves - you never see when the other player moves.  Add this code to the code block.  See the `GameStateChanged +=` code?  We are "subscribing" to the `GameStateChanged` event, so whenever that event is raised over in the Game object, our `handleGameStateChanged()` function will be invoked.
   ```csharp
    protected override void OnInitialized()
    {
        game.GameStateChanged += handleGameStateChanged;
    }

    void handleGameStateChanged()
    {
      if (other is null)
      {
            other = game.Player1 == me ? game.Player2 : game.Player1;
      }
      InvokeAsync(StateHasChanged);
    }   
   ```

#### Handle Game Over
1. At some point the game ends...how do we handle that?
   ```razor
    @if(game.Player1.Tiles.Count == game.Player2.Tiles.Count)
    {
        <p>It's a tie!</p>
    }
    else if(game.Winner == other)
    {
        <p>You lose :-(</p>
    }
    else
    {
        <p>You win!</p>
    }
    <button @onclick="playAgain">Play Again</button>   
   ```
   Inside of `playAgain()` just call `game.Reset()`, which will reset the internal game state (clear the board, reset the players, etc.) and it will also raise the `GameReset` event.  We'd better subscribe to that event so that whenever the game is reset we automatically go back to the home page.  Add this to the bottom of the `OnInitializedAsync()` method:
   ```csharp
   game.GameReset += () =>
   {
      navManager.NavigateTo("/");
   };   
   ```
   Earlier we added the `handleGameStateChanged` method as a subscriber to the `GameStateChangedEvent`.  This code shows how we can still subscribe to an event, but with a lambda function that we define right where we subscribe to the event.  This code means whenever the `GameReset` event is raised, we will automatically navigate back to the home page so we can play again.
   
### Handling Multiple Concurrent Games (i.e. Adding a Lobby)

What happens when two players are having a great time playing a game, but two other players want to play as well?  We want to have a lobby where people can see games in progress, join an existing open game, or create their own game for others to join.

#### The Lobby Class
- You need some collection that is going to keep track of all the different `Game` objects, and you need some way to be able to refer to a specific `Game` object...something like a `Dictionary<string, Game>`.  We don't want people to directly modify that collection though, so make sure you make that private.
- You need a method to let users create a new game, since our dictionary has a string for the key you could just let the user name their game and use the game name as the dictionary key.  
   ```csharp
    public static bool CreateGame(string gameName)
    {
        var newGame = new Game();        
        var gameAddedOk = games.TryAdd(gameName, newGame);
        if (gameAddedOk)
        {
            LobbyChanged?.Invoke();
            newGame.GameStateChanged += () => LobbyChanged?.Invoke();
        }
        return gameAddedOk;
    }   
   ```
  Did you notice that `LobbyChanged` line in there?  We'll want an event that other objects can subscribe to so they know if there's something new in the Lobby (this will let the Blazor page always stay up to date as different people create and join games.).  Create that event like this: `public static event Action LobbyChanged;`
- You need a way to let users get a specific game instance
   ```csharp
    public static Game? GetGame(string gameName)
    {
        return games.ContainsKey(gameName) ? games[gameName] : null;
    }   
   ```
- We'll need a way to see all the active games.  This `ActiveGames` property returns an IEnumerable of tuples, with the game name and the game object, but only of games with two players and in active play.
   ```csharp
    public static IEnumerable<(string Name, Game Game)> ActiveGames => games
        .Where(g => g.Value.Player2 is not null && g.Value.IsGameOver is false)
        .Select(g => (g.Key, g.Value));   
   ```
- We'll also want a way to get a list of all the open games
   ```csharp
   public static IEnumerable<string> OpenGameNames => games
      .Where(g => g.Value.Player2 == null)
      .Select(g => g.Key);
   ```

#### Change the Home page
The home page can no longer be where a player joins *the* game.  The home page is now the lobby that shows all the games and lets the user create a new game.  Replace `Home.razor` with the following (don't forget to check the `@using` namespace to make sure it's correct):
```razor
@page "/"
@using Lab12Demo.Logic
@inject NavigationManager navManager

<PageTitle>Dominos(ish) Lobby</PageTitle>

<h1>Dominos(ish) Lobby</h1>
@if (Lobby.OpenGameNames.Any())
{
    <h3>Choose an open game to join</h3>
    <ul>
        @foreach (var name in Lobby.OpenGameNames)
        {
            var game = Lobby.GetGame(name);
            var players = $"{game.Player1?.Name ?? "[Open]"} vs {game.Player2?.Name ?? "[Open]"}";
            <li>
               <a href="/game/@name/">@name (@players)</a>
            </li>
        }
    </ul>
}

<h3>Create a new game</h3>
<input @bind=newGameName /><button @onclick=createGame>Create Game</button>

@if(Lobby.ActiveGames.Any())
{
    <h3>Active Games</h3>
    <table style="width: 100%;" border>
        <thead>
            <tr>
                <th></th>
                <th colspan="2">Player 1</th>
                <th colspan="2">Player 2</th>
            </tr>
            <tr>
                <th>Game Name</th>
                <th>Name</th>
                <th>Tiles</th>
                <th>Name</th>
                <th>Tiles</th>
            </tr>
        </thead>
        <tbody>
            @foreach(var activeGame in Lobby.ActiveGames)
            {
                <tr>
                    <td>@activeGame.Name</td>
                    <td>@activeGame.Game.Player1?.Name</td>
                    <td>@activeGame.Game.Player1?.Tiles.Count</td>
                    <td>@activeGame.Game.Player2?.Name</td>
                    <td>@activeGame.Game.Player2?.Tiles.Count</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    string newGameName;
    void createGame()
    {
        var result = Lobby.CreateGame(newGameName);
        newGameName = null;
        StateHasChanged();
    }
    protected override void OnInitialized()
    {
        Lobby.LobbyChanged += () => InvokeAsync(StateHasChanged);
    }
}
```
A few items of note on the Home page:
   - Note how we don't list open games if there aren't any
   - Note how we don't list active games if there aren't any
   - Note how in the `OnInitialized` method we subscribe to the `LobbyChanged` event, an in our event handler we call `InvokeAsync(StateHasChanged)`.  That tells Blazor that whenever the `LobbyChanged` event is raised, to re-render the screen.  This is the bit of magic that always keeps the game lists (and player names and scores) up to date. **This is important**

#### The Game Page
Now we need a page to join a game.  Make a new file `GamePage.razor` right next to `Home.razor` and include the following content:
```razor
@page "/game/{GameName}"
@using Lab12Demo.Logic
@inject NavigationManager navManager
<PageTitle>@GameName | Dominos(ish)</PageTitle>

<h1>Game Name: @GameName</h1>

@if(game.Player2 is null)
{
    <input @bind=newPlayerName placeholder="Your Name" />
    <button @onclick=joinGame>Join Game</button>
}
else
{
    <p>Sorry, looks like the game is full</p>
    <p>@game.Player1.Name is already playing @game.Player2.Name</p>
}

@code {
    string newPlayerName;
    void joinGame()
    {
        game.Join(new Player(newPlayerName));
        navManager.NavigateTo($"/play/{GameName}/?playerName={newPlayerName}");
    }

    [Parameter]
    public string GameName{get;set;}
    Game? game;

    override protected void OnParametersSet()
    {
        game = Lobby.GetGame(GameName);
        game.GameStateChanged += ()=> InvokeAsync(StateHasChanged);
    }
}
```
A few items of note:
- Note the `@page` directive specifying this page is accessible at "/game" with a *route parameter* of `GameName` (e.g. /game/blue42). 
- Also note the `[Parameter]` attribute on the `GameName` property.  That property will be automatically filled from the URL used to access the page
- Note the `OnParametersSet()` method.  It's like `OnInitialized()` but it runs when the parameters are set (who would have guessed?).  So once we have a value for `GameName` then we can find that game from the lobby and use that game instance for the rest of the page.
- Note how we're passing the `GameName` to the "/play" page when the player joins the game.

#### Play page

The original play page needs to be tweaked a bit to play a specific game (the name of which is pulled from the URL).

1. Add the `GameName` route parameter to the `@page` directive at the top of the file: `@page "/play/{GameName}"`
1. Add the `GameName` property in the code
    ```csharp
    [Parameter]
    public string? GameName { get; set; }    
    ```
1. Add some code in the OnParametersSet() method to get the correct `game` reference.
    ```csharp
        game = Lobby.GetGame(GameName);
        if (game is null)
        {
            navManager.NavigateTo("/");
            return;
        }    
    ```

### Where are we now?

We now have an application that can host multiple concurrent games, which use events to let the lobby page know the current status of each game so as it's played other users can observe what's going on within our app, in real time.
