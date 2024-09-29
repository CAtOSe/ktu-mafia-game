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
            Base = BaseCommands.Disconnect
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
                Base = BaseCommands.Error,
                Error = ErrorMessages.UsernameTaken
            });
            return;
        }

        player.Name = username;
        _currentPlayers.Add(player);
        await player.SendMessage(new Message
        {
            Base = BaseCommands.LoggedIn
        });
        await SendPlayerList();
    }
    
    private Task SendPlayerList()
    {
        var message = new Message
        {
            Base = BaseCommands.PlayerList,
            Arguments = _currentPlayers.Select(p => p.Name).ToList(),
        };
        
        var notifications = _currentPlayers.Select(p => p.SendMessage(message));
        return Task.WhenAll(notifications);
    }
    
    public void StartGame()
    {
        if (_currentPlayers.Count < 2)
        {
            throw new InvalidOperationException("There must be at least 3 players to start the game.");
        }

        // Randomly setting Killer role to 1 player
        var random = new Random();
        int killerIndex = random.Next(_currentPlayers.Count);
        Player killer = _currentPlayers[killerIndex];
        killer.Role = "Killer";
    
        // Setting Citizen role for other players
        foreach (var player in _currentPlayers)
        {
            if (player != killer)
            {
                player.Role = "Citizen";
            }

            // Notify each player of their role
            player.SendMessage($"role-assigned:{player.Role}");
        }

        // Notify all players that the roles have been assigned
        NotifyAllPlayers(null, "roles-assigned");
    }
    
    public void NotifyAllPlayers(Player newPlayer, string action)
    {
        foreach (var player in _currentPlayers)
        {
            if (player != newPlayer && newPlayer != null)
            {
                player.SendMessage($"{action}:{newPlayer.Name}"); 
            }
        }
    }

    public List<Player> GetPlayers()
    {
        return _currentPlayers;
    }
    
    public Dictionary<string, string> GetPlayerRoles()
    {
        return _currentPlayers.ToDictionary(player => player.Name, player => player.Role);
    }

}
