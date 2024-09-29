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
}
