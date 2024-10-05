using Mafia.Server.Models;
using Mafia.Server.Models.Commands;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly List<Player> _currentPlayers = [];
    
    private bool GameStarted { get; set; } = false;
    
    public async Task DisconnectPlayer(Player player)
    {
        _currentPlayers.Remove(player);
        await player.SendMessage(new Message
        {
            Base = RequestCommands.Disconnect
        });
        player.CloseConnection();

        if (_currentPlayers.Count == 0)
        {
            ResetGame();
        }
        else
        { 
            await SendPlayerList();
        }
    }
    
    public async Task TryAddPlayer(Player player, string username)
    {
        if (player.IsLoggedIn)
        {
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.Error,
                Error = ErrorMessages.AlreadyLoggedIn,
            });
            return;
        }
        
        var usernameTaken = _currentPlayers.Any(p =>
            p.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (usernameTaken)
        {
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.Error,
                Error = ErrorMessages.UsernameTaken
            });
            return;
        }

        player.IsLoggedIn = true;
        player.Name = username;
        player.IsHost = _currentPlayers.Count == 0;
        
        _currentPlayers.Add(player);
        await player.SendMessage(new Message
        {
            Base = ResponseCommands.LoggedIn,
            Arguments = player.IsHost ? ["host"] : null
        });
        await SendPlayerList();
    }
    
    private Task SendPlayerList()
    {
        var message = new Message
        {
            Base = ResponseCommands.PlayerListUpdate,
            Arguments = _currentPlayers.Select(p => p.Name).ToList(),
        };
        return NotifyAllPlayers(message);
    }
    
    public async Task StartGame()
    {
        if (GameStarted)
        {
            Console.WriteLine("Game already started");
            return;
        }
        
        if (_currentPlayers.Count < 2)
        {
            throw new InvalidOperationException("There must be at least 3 players to start the game.");
        }

        Console.WriteLine("Assigning roles and starting the countdown.");
        
        await NotifyAllPlayers(new Message
        {
            Base = ResponseCommands.StartCountdown,
            Arguments = [GameConfiguration.BeginCountdown.ToString()]
        });
        var gameStartTask = Task.Delay(GameConfiguration.BeginCountdown).ContinueWith(async t =>
        {
            await NotifyAllPlayers(new Message
            {
                Base = ResponseCommands.GameStarted,
            });
        });

        // Randomly setting Killer role to 1 player
        var random = new Random();
        var killerIndex = random.Next(_currentPlayers.Count);
        var killer = _currentPlayers[killerIndex];
        killer.Role = PlayerRole.Killer;
    
        // Setting Citizen role for other players
        foreach (var player in _currentPlayers)
        {
            if (player != killer)
            {
                player.Role = PlayerRole.Citizen;
            }

            // Notify each player of their role
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.RoleAssigned,
                Arguments = [player.RoleName]
            });
        }

        await gameStartTask;
    }

    public List<Player> GetPlayers()
    {
        return _currentPlayers;
    }
    
    public Dictionary<string, string> GetPlayerRoles()
    {
        return _currentPlayers.ToDictionary(player => player.Name, player => player.RoleName);
    }
    
    private Task NotifyAllPlayers(Message message)
    {
        var notifications = _currentPlayers.Select(p => p.SendMessage(message));
        return Task.WhenAll(notifications);
    }

    private void ResetGame()
    {
        GameStarted = false;
    }
}
