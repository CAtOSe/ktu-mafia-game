using Mafia.Server.Models;
using Mafia.Server.Models.Commands;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly List<Player> _currentPlayers = [];
    public async Task DisconnectPlayer(Player player)
    {
        _currentPlayers.Remove(player);
        await player.SendMessage(new Message
        {
            Base = RequestCommands.Disconnect
        });
        await SendPlayerList();
        player.CloseConnection();
    }
    
    public async Task TryAddPlayer(Player player, string username)
    {
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

        player.Name = username;
        _currentPlayers.Add(player);
        await player.SendMessage(new Message
        {
            Base = ResponseCommands.LoggedIn
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
        if (_currentPlayers.Count < 2)
        {
            throw new InvalidOperationException("There must be at least 3 players to start the game.");
        }

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

        // Notify all players that the roles have been assigned
        await NotifyAllPlayers(new Message
        {
            Base = ResponseCommands.GameStarted
        });
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
}
